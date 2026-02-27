using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
using WPF_LoginForm.Utils;

namespace WPF_LoginForm.Views
{
    public partial class GerenciarDados : Window
    {
        public ObservableCollection<ItemGerenciaModel> ListaGeral { get; set; }
        private string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";
        //@"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public GerenciarDados()
        {
            InitializeComponent();
            ListaGeral = new ObservableCollection<ItemGerenciaModel>();
            dgGerenciamento.ItemsSource = ListaGeral;
            CarregarTudoDoBanco();
        }

        private void CarregarTudoDoBanco()
        {
            try
            {
                ListaGeral.Clear();
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();

                    // 1. CARREGA EQUIPAMENTOS
                    string sqlEquip = "SELECT Nome, Tipo, Comodo FROM Equipamentos";
                    using (SqlCommand cmd = new SqlCommand(sqlEquip, con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        while (dr.Read()) ListaGeral.Add(new ItemGerenciaModel
                        {
                            Nome = dr["Nome"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            Comodo = dr["Comodo"].ToString(),
                            Categoria = "EQUIPAMENTO"
                        });

                    // 2. CARREGA USUÁRIOS
                    string sqlUser = "SELECT Nome, Sobrenome, Email, Telefone, NivelAcesso FROM Usuarios";
                    using (SqlCommand cmd = new SqlCommand(sqlUser, con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        while (dr.Read()) ListaGeral.Add(new ItemGerenciaModel
                        {
                            Nome = dr["Nome"].ToString(),
                            Sobrenome = dr["Sobrenome"].ToString(),
                            Comodo = dr["Email"].ToString(),
                            Telefone = dr["Telefone"].ToString(),
                            Tipo = dr["NivelAcesso"].ToString(),
                            Categoria = "USUÁRIO"
                        });
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
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
                        // Valida Senha Antiga antes de salvar Usuário
                        string sqlCheck = "SELECT Senha FROM Usuarios WHERE Nome = @o";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, con);
                        cmdCheck.Parameters.AddWithValue("@o", item.Nome);
                        if (cmdCheck.ExecuteScalar()?.ToString() != HashHelper.GerarHash(txtSenhaAntiga.Password))
                        {
                            MessageBox.Show("Senha antiga incorreta!"); return;
                        }
                        // Update Usuário
                        string sqlU = "UPDATE Usuarios SET Nome=@n, Sobrenome=@s, Email=@e, Telefone=@t, NivelAcesso=@a, Senha=@p WHERE Nome=@o";
                        SqlCommand cmd = new SqlCommand(sqlU, con);
                        cmd.Parameters.AddWithValue("@p", HashHelper.GerarHash(txtSenhaNova.Password));
                        cmd.Parameters.AddWithValue("@s", txtSobrenome.Text);
                        cmd.Parameters.AddWithValue("@e", txtComodo.Text);
                        cmd.Parameters.AddWithValue("@t", txtTelefone.Text);
                        cmd.Parameters.AddWithValue("@a", txtTipo.Text);
                        cmd.Parameters.AddWithValue("@n", txtNome.Text);
                        cmd.Parameters.AddWithValue("@o", item.Nome);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Update Equipamento (Sem senha)
                        string sqlE = "UPDATE Equipamentos SET Nome=@n, Tipo=@t, Comodo=@c WHERE Nome=@o";
                        SqlCommand cmd = new SqlCommand(sqlE, con);
                        cmd.Parameters.AddWithValue("@n", txtNome.Text);
                        cmd.Parameters.AddWithValue("@t", txtTipo.Text);
                        cmd.Parameters.AddWithValue("@c", txtComodo.Text);
                        cmd.Parameters.AddWithValue("@o", item.Nome);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Salvo com sucesso!");
                    CarregarTudoDoBanco();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}