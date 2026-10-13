
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace BaiTap47_MDI
{
    public class FormProduct : Form
    {
        private BindingList<Product> products = new BindingList<Product>();
        private BindingSource source = new BindingSource();
        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtSearch;
        private DataGridView dgv;

        public FormProduct()
        {
            Text = "Quản lý Sản phẩm";
            Width = 900;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            Label lblCode = new Label { Text = "Mã sản phẩm", Left = 15, Top = 20, Width = 100 };
            txtCode = new TextBox { Left = 120, Top = 16, Width = 180 };

            Label lblName = new Label { Text = "Tên sản phẩm", Left = 320, Top = 20, Width = 100 };
            txtName = new TextBox { Left = 420, Top = 16, Width = 200 };

            Label lblPrice = new Label { Text = "Đơn giá", Left = 640, Top = 20, Width = 60 };
            txtPrice = new TextBox { Left = 700, Top = 16, Width = 140 };

            Button btnAdd = new Button { Text = "Thêm", Left = 15, Top = 55, Width = 90 };
            Button btnEdit = new Button { Text = "Sửa", Left = 115, Top = 55, Width = 90 };
            Button btnDelete = new Button { Text = "Xóa", Left = 215, Top = 55, Width = 90 };
            Button btnClear = new Button { Text = "Làm mới", Left = 315, Top = 55, Width = 90 };
            Button btnSearch = new Button { Text = "Tìm kiếm", Left = 600, Top = 55, Width = 90 };

            txtSearch = new TextBox { Left = 410, Top = 57, Width = 180 };

            dgv = new DataGridView
            {
                Left = 15,
                Top = 100,
                Width = 825,
                Height = 380,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            source.DataSource = products;
            dgv.DataSource = source;

            btnAdd.Click += (s, e) => AddProduct();
            btnEdit.Click += (s, e) => EditProduct();
            btnDelete.Click += (s, e) => DeleteProduct();
            btnClear.Click += (s, e) => ClearInputs();
            btnSearch.Click += (s, e) => SearchProducts();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is Product p)
                {
                    txtCode.Text = p.Code;
                    txtName.Text = p.Name;
                    txtPrice.Text = p.Price.ToString();
                }
            };

            dgv.CellMouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dgv.ClearSelection();
                    dgv.Rows[e.RowIndex].Selected = true;
                    dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[0];
                }
            };

            ContextMenuStrip context = new ContextMenuStrip();
            context.Items.Add("Xóa dòng", null, (s, e) => DeleteProduct());
            context.Items.Add("Làm mới dữ liệu", null, (s, e) =>
            {
                source.DataSource = products;
                source.ResetBindings(false);
                UpdateMainStatus("Đã làm mới danh sách sản phẩm.");
            });
            context.Items.Add("Xem chi tiết", null, (s, e) => ShowDetails());
            dgv.ContextMenuStrip = context;

            Controls.AddRange(new Control[]
            {
                lblCode, txtCode, lblName, txtName, lblPrice, txtPrice,
                btnAdd, btnEdit, btnDelete, btnClear, txtSearch, btnSearch, dgv
            });
        }

        private void AddProduct()
        {
            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();

            if (code == "" || name == "")
            {
                MessageBox.Show("Vui lòng nhập mã và tên sản phẩm.");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ.");
                return;
            }

            if (products.Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại.");
                return;
            }

            products.Add(new Product { Code = code, Name = name, Price = price });
            source.ResetBindings(false);
            ClearInputs();
            UpdateMainStatus("Thêm sản phẩm thành công.");
        }

        private void EditProduct()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Product selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.");
                return;
            }

            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();

            if (code == "" || name == "" ||
                !decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Vui lòng nhập thông tin hợp lệ.");
                return;
            }

            if (products.Any(x => x != selected &&
                x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại.");
                return;
            }

            int index = products.IndexOf(selected);
            products[index] = new Product { Code = code, Name = name, Price = price };
            source.ResetBindings(false);
            UpdateMainStatus("Sửa sản phẩm thành công.");
        }

        private void DeleteProduct()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Product selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            products.Remove(selected);
            source.ResetBindings(false);
            ClearInputs();
            UpdateMainStatus("Xóa sản phẩm thành công.");
        }

        private void SearchProducts()
        {
            string keyword = txtSearch.Text.Trim();

            source.DataSource = new BindingList<Product>(
                products.Where(x =>
                    x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList());

            UpdateMainStatus("Đã tìm kiếm sản phẩm.");
        }

        private void ShowDetails()
        {
            if (dgv.CurrentRow?.DataBoundItem is Product p)
            {
                MessageBox.Show(
                    $"Mã sản phẩm: {p.Code}\nTên sản phẩm: {p.Name}\nĐơn giá: {p.Price:N0}",
                    "Chi tiết sản phẩm");
            }
        }

        private void ClearInputs()
        {
            txtCode.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtSearch.Clear();
            source.DataSource = products;
            source.ResetBindings(false);
            dgv.ClearSelection();
        }

        private void UpdateMainStatus(string message)
        {
            if (MdiParent is FormMain main)
                main.UpdateStatus(message);
        }
    }

    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
