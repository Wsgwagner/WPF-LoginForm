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

            // Lógica de Seleção
            dgGerenciamento.SelectionChanged += (s, e) => {
                var item = dgGerenciamento.SelectedItem as ItemGerenciaModel;
                if (item != null)
                {
                    pnlSeguranca.Visibility = (item.Categoria == "USUÁRIO") ? Visibility.Visible : Visibility.Collapsed;
                    cbStatus.IsEnabled = (item.Categoria == "EQUIPAMENTO");
                    cbStatus.Text = item.Status;
                }
            };

            this.Loaded += (s, e) => {
                if (SessaoSistema.ResidenciaId <= 0) SessaoSistema.ResidenciaId = 1;
                CarregarDadosBanco();
                CarregarDadosResidencia();
            };
        }

        private void CarregarDadosBanco()
        {
            try
            {
                ListaGeral.Clear();
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = @"
                        SELECT Id, Nome, Tipo, Comodo, Status, 'EQUIPAMENTO' as Cat, '' as Pwd FROM Equipamentos WHERE ResidenciaId = @rid
                        UNION
                        SELECT Id, Name, AccessLevel, Email, 'On-line', 'USUÁRIO' as Cat, Password FROM Users WHERE ResidenciaId = @rid";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read()) ListaGeral.Add(new ItemGerenciaModel
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                Nome = dr["Nome"].ToString(),
                                Tipo = dr["Tipo"].ToString(),
                                Comodo = dr["Comodo"].ToString(),
                                Status = dr["Status"].ToString(),
                                Categoria = dr["Cat"].ToString(),
                                Senha = dr["Pwd"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                        // VALIDAÇÃO DE SENHA (SÓ SALVA SE A ANTIGA FOR IGUAL AO HASH)
                        if (!string.IsNullOrWhiteSpace(txtSenhaNova.Password))
                        {
                            if (HashHelper.GerarHash(txtSenhaAntiga.Password) != item.Senha)
                            {
                                MessageBox.Show("Senha atual incorreta!"); return;
                            }
                            item.Senha = HashHelper.GerarHash(txtSenhaNova.Password);
                        }
                        string sql = "UPDATE Users SET Name=@n, AccessLevel=@t, Password=@p WHERE Id=@id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@n", txtNome.Text);
                            cmd.Parameters.AddWithValue("@t", txtTipo.Text);
                            cmd.Parameters.AddWithValue("@p", item.Senha);
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // SALVA O NOVO STATUS SELECIONADO NO COMBOBOX
                        string novoStatus = cbStatus.Text;
                        string sql = "UPDATE Equipamentos SET Nome=@n, Tipo=@t, Comodo=@c, Status=@s WHERE Id=@id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@n", txtNome.Text);
                            cmd.Parameters.AddWithValue("@t", txtTipo.Text);
                            cmd.Parameters.AddWithValue("@c", txtComodo.Text);
                            cmd.Parameters.AddWithValue("@s", novoStatus);
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("IA HOME: Dados e Senha atualizados!");
                    CarregarDadosBanco();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DeleteAction(object sender, RoutedEventArgs e)
        {
            var item = dgGerenciamento.SelectedItem as ItemGerenciaModel;
            if (item == null) return;
            if (MessageBox.Show($"Excluir {item.Nome}?", "IA HOME", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"DELETE FROM {(item.Categoria == "USUÁRIO" ? "Users" : "Equipamentos")} WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", item.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                CarregarDadosBanco();
            }
        }

        private void SaveResidencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE Residencias SET NomeResidencia=@n, Endereco=@e, Cidade=@c WHERE Id=@rid";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@n", txtResNome.Text);
                        cmd.Parameters.AddWithValue("@e", txtResEnd.Text);
                        cmd.Parameters.AddWithValue("@c", txtResCidade.Text);
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("IA HOME: Residência atualizada!");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CarregarDadosResidencia()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT NomeResidencia, Endereco, Cidade FROM Residencias WHERE Id = @rid";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                            if (dr.Read())
                            {
                                txtResNome.Text = dr["NomeResidencia"].ToString();
                                txtResEnd.Text = dr["Endereco"].ToString();
                                txtResCidade.Text = dr["Cidade"].ToString();
                            }
                    }
                }
            }
            catch { }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}