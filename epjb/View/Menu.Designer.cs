// Layout gerado pelo Windows Forms Designer; os eventos e regras estão no arquivo .cs da tela.
namespace epjb.View
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUsuario = new System.Windows.Forms.Label();
            this.dgvMensagens = new System.Windows.Forms.DataGridView();
            this.txtConteudo = new System.Windows.Forms.TextBox();
            this.btnPostar = new System.Windows.Forms.Button();
            this.cmbUsuarios = new System.Windows.Forms.ComboBox();
            this.btnSeguir = new System.Windows.Forms.Button();
            this.lstSeguindo = new System.Windows.Forms.ListBox();
            this.btnDeletar = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMensagens)).BeginInit();
            this.SuspendLayout();

            // lblUsuario
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.Location = new System.Drawing.Point(10, 10);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(100, 25);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Bem-vindo!";

            // dgvMensagens
            this.dgvMensagens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMensagens.Location = new System.Drawing.Point(10, 45);
            this.dgvMensagens.Name = "dgvMensagens";
            this.dgvMensagens.Size = new System.Drawing.Size(600, 300);
            this.dgvMensagens.TabIndex = 1;

            // txtConteudo
            this.txtConteudo.Location = new System.Drawing.Point(10, 380);
            this.txtConteudo.Multiline = true;
            this.txtConteudo.Name = "txtConteudo";
            this.txtConteudo.Size = new System.Drawing.Size(500, 60);
            this.txtConteudo.TabIndex = 2;

            // btnPostar
            this.btnPostar.Location = new System.Drawing.Point(520, 380);
            this.btnPostar.Name = "btnPostar";
            this.btnPostar.Size = new System.Drawing.Size(90, 60);
            this.btnPostar.TabIndex = 3;
            this.btnPostar.Text = "Postar";
            this.btnPostar.UseVisualStyleBackColor = true;
            this.btnPostar.Click += new System.EventHandler(this.btnPostar_Click);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(620, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Seguir Usuários:";

            // cmbUsuarios
            this.cmbUsuarios.FormattingEnabled = true;
            this.cmbUsuarios.Location = new System.Drawing.Point(620, 65);
            this.cmbUsuarios.Name = "cmbUsuarios";
            this.cmbUsuarios.Size = new System.Drawing.Size(150, 23);
            this.cmbUsuarios.TabIndex = 5;

            // btnSeguir
            this.btnSeguir.Location = new System.Drawing.Point(620, 95);
            this.btnSeguir.Name = "btnSeguir";
            this.btnSeguir.Size = new System.Drawing.Size(150, 30);
            this.btnSeguir.TabIndex = 6;
            this.btnSeguir.Text = "Seguir";
            this.btnSeguir.UseVisualStyleBackColor = true;
            this.btnSeguir.Click += new System.EventHandler(this.btnSeguir_Click);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(620, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Meus Seguindo:";

            // lstSeguindo
            this.lstSeguindo.FormattingEnabled = true;
            this.lstSeguindo.ItemHeight = 15;
            this.lstSeguindo.Location = new System.Drawing.Point(620, 165);
            this.lstSeguindo.Name = "lstSeguindo";
            this.lstSeguindo.Size = new System.Drawing.Size(150, 180);
            this.lstSeguindo.TabIndex = 8;

            // btnDeletar
            this.btnDeletar.Location = new System.Drawing.Point(10, 450);
            this.btnDeletar.Name = "btnDeletar";
            this.btnDeletar.Size = new System.Drawing.Size(90, 30);
            this.btnDeletar.TabIndex = 9;
            this.btnDeletar.Text = "Deletar";
            this.btnDeletar.UseVisualStyleBackColor = true;
            this.btnDeletar.Click += new System.EventHandler(this.btnDeletar_Click);

            // btnAtualizar
            this.btnAtualizar.Location = new System.Drawing.Point(105, 450);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(90, 30);
            this.btnAtualizar.TabIndex = 10;
            this.btnAtualizar.Text = "Atualizar";
            this.btnAtualizar.UseVisualStyleBackColor = true;
            this.btnAtualizar.Click += new System.EventHandler(this.btnAtualizar_Click);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 360);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Nova Mensagem:";

            // btnLogout
            this.btnLogout.Location = new System.Drawing.Point(620, 450);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(150, 30);
            this.btnLogout.TabIndex = 12;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // Menu
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAtualizar);
            this.Controls.Add(this.btnDeletar);
            this.Controls.Add(this.lstSeguindo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSeguir);
            this.Controls.Add(this.cmbUsuarios);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPostar);
            this.Controls.Add(this.txtConteudo);
            this.Controls.Add(this.dgvMensagens);
            this.Controls.Add(this.lblUsuario);
            this.Name = "Menu";
            this.Text = "Twitter Simplificado";
            this.Load += new System.EventHandler(this.Menu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMensagens)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.DataGridView dgvMensagens;
        private System.Windows.Forms.TextBox txtConteudo;
        private System.Windows.Forms.Button btnPostar;
        private System.Windows.Forms.ComboBox cmbUsuarios;
        private System.Windows.Forms.Button btnSeguir;
        private System.Windows.Forms.ListBox lstSeguindo;
        private System.Windows.Forms.Button btnDeletar;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnAtualizar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
