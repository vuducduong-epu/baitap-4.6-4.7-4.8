using System;
using System.Windows.Forms;

namespace BaiTap47_MDI
{
    public class FormMain : Form
    {
        private ToolStripStatusLabel lblUser;
        private ToolStripStatusLabel lblMessage;
        private ToolStripStatusLabel lblTime;
        private System.Windows.Forms.Timer timer;

        public FormMain()
        {
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            Text = "Hệ thống quản lý";

            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem heThong = new ToolStripMenuItem("Hệ thống");
            heThong.DropDownItems.Add("Đăng xuất", null, (s, e) =>
            {
                MessageBox.Show("Đã đăng xuất.");
                Close();
            });
            heThong.DropDownItems.Add("Thoát", null, (s, e) => Close());

            ToolStripMenuItem danhMuc = new ToolStripMenuItem("Danh mục");
            danhMuc.DropDownItems.Add("Quản lý Sinh viên", null,
                (s, e) => OpenChild<FormStudent>());
            danhMuc.DropDownItems.Add("Quản lý Sản phẩm", null,
                (s, e) => OpenChild<FormProduct>());

            ToolStripMenuItem cuaSo = new ToolStripMenuItem("Cửa sổ");
            cuaSo.DropDownItems.Add("Cascade", null,
                (s, e) => LayoutMdi(MdiLayout.Cascade));
            cuaSo.DropDownItems.Add("Tile Horizontal", null,
                (s, e) => LayoutMdi(MdiLayout.TileHorizontal));
            cuaSo.DropDownItems.Add("Tile Vertical", null,
                (s, e) => LayoutMdi(MdiLayout.TileVertical));

            menu.Items.Add(heThong);
            menu.Items.Add(danhMuc);
            menu.Items.Add(cuaSo);
            MainMenuStrip = menu;
            Controls.Add(menu);

            ToolStrip tool = new ToolStrip();
            tool.Items.Add("Sinh viên", null,
                (s, e) => OpenChild<FormStudent>());
            tool.Items.Add("Sản phẩm", null,
                (s, e) => OpenChild<FormProduct>());
            Controls.Add(tool);

            StatusStrip status = new StatusStrip();
            lblUser = new ToolStripStatusLabel("Người dùng: Admin");
            lblMessage = new ToolStripStatusLabel("Sẵn sàng");
            lblTime = new ToolStripStatusLabel();

            status.Items.Add(lblUser);
            status.Items.Add(new ToolStripStatusLabel("|"));
            status.Items.Add(lblMessage);
            status.Items.Add(new ToolStripStatusLabel { Spring = true });
            status.Items.Add(lblTime);
            Controls.Add(status);

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
                lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            timer.Start();
        }

        private void OpenChild<T>() where T : Form, new()
        {
            foreach (Form child in MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    return;
                }
            }

            T form = new T();
            form.MdiParent = this;
            form.Show();
        }

        public void UpdateStatus(string message)
        {
            lblMessage.Text = message;
        }
    }
}
