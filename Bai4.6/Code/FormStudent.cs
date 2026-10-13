
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaiTap47_MDI
{
    public class FormStudent : Form
    {

        private BindingList<Student> students = new BindingList<Student>();
        private BindingSource source = new BindingSource();
        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtClass;
        private TextBox txtSearch;
        private DataGridView dgv;

        public FormStudent()
        {
            Text = "Quản lý Sinh viên";
            Width = 900;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            Label lblCode = new Label { Text = "Mã sinh viên", Left = 15, Top = 20, Width = 100 };
            txtCode = new TextBox { Left = 120, Top = 16, Width = 180 };

            Label lblName = new Label { Text = "Họ và tên", Left = 320, Top = 20, Width = 100 };
            txtName = new TextBox { Left = 420, Top = 16, Width = 200 };

            Label lblClass = new Label { Text = "Lớp", Left = 640, Top = 20, Width = 50 };
            txtClass = new TextBox { Left = 690, Top = 16, Width = 150 };

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

            source.DataSource = students;
            dgv.DataSource = source;

            btnAdd.Click += (s, e) => AddStudent();
            btnEdit.Click += (s, e) => EditStudent();
            btnDelete.Click += (s, e) => DeleteStudent();
            btnClear.Click += (s, e) => ClearInputs();
            btnSearch.Click += (s, e) => SearchStudents();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is Student st)
                {
                    txtCode.Text = st.Code;
                    txtName.Text = st.Name;
                    txtClass.Text = st.ClassName;
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
            context.Items.Add("Xóa dòng", null, (s, e) => DeleteStudent());
            context.Items.Add("Làm mới dữ liệu", null, (s, e) =>
            {
                source.DataSource = students;
                source.ResetBindings(false);
                UpdateMainStatus("Đã làm mới danh sách sinh viên.");
            });
            context.Items.Add("Xem chi tiết", null, (s, e) => ShowDetails());
            dgv.ContextMenuStrip = context;

            Controls.AddRange(new Control[]
            {
                lblCode, txtCode, lblName, txtName, lblClass, txtClass,
                btnAdd, btnEdit, btnDelete, btnClear, txtSearch, btnSearch, dgv
            });
        }

        private void AddStudent()
        {
            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();
            string className = txtClass.Text.Trim();

            if (code == "" || name == "" || className == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mã sinh viên, họ tên và lớp.");
                return;
            }

            if (students.Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.");
                return;
            }

            students.Add(new Student { Code = code, Name = name, ClassName = className });
            source.ResetBindings(false);
            ClearInputs();
            UpdateMainStatus("Thêm sinh viên thành công.");
        }

        private void EditStudent()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa.");
                return;
            }

            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();
            string className = txtClass.Text.Trim();

            if (code == "" || name == "" || className == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (students.Any(x => x != selected &&
                x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.");
                return;
            }

            selected.Code = code;
            selected.Name = name;
            selected.ClassName = className;

            int index = students.IndexOf(selected);
            students[index] = new Student
            {
                Code = code,
                Name = name,
                ClassName = className
            };

            source.ResetBindings(false);
            UpdateMainStatus("Sửa thông tin sinh viên thành công.");
        }

        private void DeleteStudent()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            students.Remove(selected);
            source.ResetBindings(false);
            ClearInputs();
            UpdateMainStatus("Xóa sinh viên thành công.");
        }

        private void SearchStudents()
        {
            string keyword = txtSearch.Text.Trim();

            source.DataSource = new BindingList<Student>(
                students.Where(x =>
                    x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.ClassName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList());

            UpdateMainStatus("Đã tìm kiếm sinh viên.");
        }

        private void ShowDetails()
        {
            if (dgv.CurrentRow?.DataBoundItem is Student st)
            {
                MessageBox.Show(
                    $"Mã sinh viên: {st.Code}\nHọ và tên: {st.Name}\nLớp: {st.ClassName}",
                    "Chi tiết sinh viên");
            }
        }

        private void ClearInputs()
        {
            txtCode.Clear();
            txtName.Clear();
            txtClass.Clear();
            txtSearch.Clear();
            source.DataSource = students;
            source.ResetBindings(false);
            dgv.ClearSelection();
        }

        private void UpdateMainStatus(string message)
        {
            if (MdiParent is FormMain main)
                main.UpdateStatus(message);
        }
    }

    public class Student
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
    }
}
