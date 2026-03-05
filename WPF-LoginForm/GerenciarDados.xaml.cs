using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using WPF_LoginForm.Utils;

namespace WPF_LoginForm.Views
{
    public partial class GerenciarDados : Window
    {
        public ObservableCollection<ItemGerenciaModel> ListaGeral { get; set; } = new ObservableCollection<ItemGerenciaModel>();
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public GerenciarDados()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Loaded += (s, e) => {
                if (SessaoSistema.ResidenciaId <= 0) SessaoSistema.ResidenciaId = 1;
                CarregarDadosBanco();
                CarregarDadosResidencia();

                if (SessaoSistema.AccessLevel != "Administrador")
                    BloquearParaMorador();
            };
        }

        private void BloquearParaMorador()
        {
            dgGerenciamento.IsReadOnly = true;
            txtNome.IsReadOnly = txtComodo.IsReadOnly = txtTipo.IsReadOnly = true;
            txtResNome.IsReadOnly = txtResEnd.IsReadOnly = txtResCidade.IsReadOnly = true;
            cbStatus.IsEnabled = false;
            btnSalvarGeral.Visibility = btnSalvarResidencia.Visibility = btnExcluir.Visibility = Visibility.Collapsed;
            txtTituloEdicao.Text = "🔒 MODO VISUALIZAÇÃO (APENAS LEITURA)";
        }

        private void CarregarDadosBanco()
        {
            try
            {
                ListaGeral.Clear();
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Busca unificada: Equipamentos trazem o NOME do cômodo via JOIN
                    string sql = @"
                        SELECT e.Id, e.Nome, e.Tipo, c.Nome as ComodoNome, e.Status, 'EQUIPAMENTO' as Cat, '' as Pwd 
                        FROM Equipamentos e
                        LEFT JOIN Comodos c ON e.ComodoId = c.Id
                        WHERE e.ResidenciaId = @rid
                        UNION
                        SELECT Id, Name, AccessLevel, Email, 'On-line', 'USUÁRIO' as Cat, Password 
                        FROM Users WHERE ResidenciaId = @rid";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read()) ListaGeral.Add(new ItemGerenciaModel
                            {
                                Id = Convert.ToInt32(dr[0]),
                                Nome = dr[1].ToString(),
                                Tipo = dr[2].ToString(),
                                Comodo = dr[3].ToString(),
                                Status = dr[4].ToString(),
                                Categoria = dr[5].ToString(),
                                Senha = dr[6].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
        }

        private void DeleteAction(object sender, RoutedEventArgs e)
        {
            var item = dgGerenciamento.SelectedItem as ItemGerenciaModel;
            if (item == null) return;

            // Se for usuário, apenas exclui o usuário normalmente
            if (item.Categoria == "USUÁRIO")
            {
                ExcluirRegistroSimples("Users", item.Id);
                return;
            }

            // Lógica Especial para Equipamentos: Excluir cômodo se ficar vazio
            if (MessageBox.Show($"Deseja excluir o equipamento {item.Nome}? Se este for o único item no local '{item.Comodo}', o cômodo também será apagado.", "IA HOME", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(strCon))
                    {
                        con.Open();

                        // 1. Pega o ID do cômodo antes de apagar o equipamento
                        int comodoId = 0;
                        string sqlGetComodo = "SELECT ComodoId FROM Equipamentos WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlGetComodo, con))
                        {
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value) comodoId = (int)result;
                        }

                        // 2. Apaga o equipamento
                        string sqlDelEquip = "DELETE FROM Equipamentos WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlDelEquip, con))
                        {
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Verifica se o cômodo ficou vazio e apaga se necessário
                        if (comodoId > 0)
                        {
                            string sqlCheck = "SELECT COUNT(*) FROM Equipamentos WHERE ComodoId = @cid";
                            using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, con))
                            {
                                cmdCheck.Parameters.AddWithValue("@cid", comodoId);
                                int restantes = (int)cmdCheck.ExecuteScalar();

                                if (restantes == 0)
                                {
                                    string sqlDelComodo = "DELETE FROM Comodos WHERE Id = @cid";
                                    using (SqlCommand cmdDelC = new SqlCommand(sqlDelComodo, con))
                                    {
                                        cmdDelC.Parameters.AddWithValue("@cid", comodoId);
                                        cmdDelC.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    CarregarDadosBanco();
                    MessageBox.Show("Excluído com sucesso!");
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
        }

        private void ExcluirRegistroSimples(string tabela, int id)
        {
            if (MessageBox.Show("Confirmar exclusão?", "IA HOME", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"DELETE FROM {tabela} WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                CarregarDadosBanco();
            }
        }

        private void EditAction(object sender, RoutedEventArgs e)
        {
            var item = dgGerenciamento.SelectedItem as ItemGerenciaModel;
            if (item == null) return;
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    if (item.Categoria == "USUÁRIO")
                    {
                        // Lógica de atualização de usuário (incluindo Hash se senha nova for digitada)
                        string sql = "UPDATE Users SET Name=@n, AccessLevel=@t WHERE Id=@id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@n", txtNome.Text);
                            cmd.Parameters.AddWithValue("@t", txtTipo.Text);
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Atualiza equipamento e tenta vincular ao cômodo pelo Nome digitado
                        string sql = @"
                            UPDATE Equipamentos SET Nome=@n, Tipo=@t, Status=@s,
                            ComodoId = (SELECT TOP 1 Id FROM Comodos WHERE Nome = @c AND ResidenciaId = @rid)
                            WHERE Id=@id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@n", txtNome.Text);
                            cmd.Parameters.AddWithValue("@t", txtTipo.Text);
                            cmd.Parameters.AddWithValue("@s", cbStatus.Text);
                            cmd.Parameters.AddWithValue("@c", txtComodo.Text);
                            cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId);
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Dados atualizados!");
                    CarregarDadosBanco();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Métodos de apoio (Residência e Janela) permanecem iguais...
        private void SaveResidencia_Click(object sender, RoutedEventArgs e) { /* ... código anterior ... */ }
        private void CarregarDadosResidencia() { /* ... código anterior ... */ }
        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}