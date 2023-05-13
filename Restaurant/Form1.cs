using System.Data.SqlClient;

namespace Restaurant;

public partial class Form1 : Form
{
    
    public Form1()
    {
        InitializeComponent();
        SqlConnection conn = new SqlConnection("Data Source=ServerName;" +
                                               "Initial" +
                                               "Catalog=DatabaseName;User ID=UserName;Password=Password");
    }
    private void btnAddCustomer_Click(object sender, EventArgs e)
    {
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Customers (Name, Phone, Email) VALUES (@Name, @Phone, @Email)", conn);
            cmd.Parameters.AddWithValue("@Name", txtName.Text);
            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Customer added successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    private void btnViewCustomers_Click(object sender, EventArgs e)
    {
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Customers", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvCustomers.DataSource = dt;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    private void btnViewMenu_Click(object sender, EventArgs e)
    {
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM MenuItems", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvMenu.DataSource = dt;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    private void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (@CustomerId, @OrderDate, @TotalAmount); SELECT SCOPE_IDENTITY()", conn);
            cmd.Parameters.AddWithValue("@CustomerId", txtCustomerId.Text);
            cmd.Parameters.AddWithValue("@OrderDate", DateTime.Today);
            cmd.Parameters.AddWithValue("@TotalAmount", txtTotalAmount.Text);
            int orderId = Convert.ToInt32(cmd.ExecuteScalar());
            foreach (DataGridViewRow row in dgvOrderItems.Rows)
            {
                int menuItemId = Convert.ToInt32(row.Cells["MenuItemId"].Value);
                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                decimal amount = quantity * price;
                SqlCommand cmd2 = new SqlCommand("INSERT INTO OrderItems (OrderId, MenuItemId, Quantity, Price, Amount) VALUES (@OrderId, @MenuItemId, @Quantity, @Price, @Amount)", conn);
                cmd2.Parameters.AddWithValue("@OrderId", orderId);
                cmd2.Parameters.AddWithValue("@MenuItemId", menuItemId);
                cmd2.Parameters.AddWithValue("@Quantity", quantity);
                cmd2.Parameters.AddWithValue("@Price", price);
                cmd2.Parameters.AddWithValue("@Amount", amount);
                cmd2.ExecuteNonQuery();
            }
            MessageBox.Show("Order placed successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    private void btnAddToOrder_Click(object sender, EventArgs e)
    {
        try
        {
            int menuItemId = Convert.ToInt32(dgvMenu.SelectedRows[0].Cells["MenuItemId"].Value);
            string itemName = dgvMenu.SelectedRows[0].Cells["ItemName"].Value.ToString();
            decimal price = Convert.ToDecimal(dgvMenu.SelectedRows[0].Cells["Price"].Value);
            int quantity = Convert.ToInt32(numQuantity.Value);
            decimal amount = quantity * price;
            dgvOrderItems.Rows.Add(menuItemId, itemName, quantity, price, amount);
            decimal totalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            totalAmount += amount;
            txtTotalAmount.Text = totalAmount.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
    private void btnRemoveFromOrder_Click(object sender, EventArgs e)
    {
        try
        {
            int rowIndex = dgvOrderItems.CurrentCell.RowIndex;
            decimal amount = Convert.ToDecimal(dgvOrderItems.Rows[rowIndex].Cells["Amount"].Value);
            decimal totalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            totalAmount -= amount;
            txtTotalAmount.Text = totalAmount.ToString();
            dgvOrderItems.Rows.RemoveAt(rowIndex);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
    private void btnSaveOrder_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Orders (TableNumber, OrderDate, TotalAmount) VALUES (@TableNumber, @OrderDate, @TotalAmount); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TableNumber", cmbTables.SelectedItem);
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TotalAmount", txtTotalAmount.Text);
                int orderId = Convert.ToInt32(cmd.ExecuteScalar());
                foreach (DataGridViewRow row in dgvOrderItems.Rows)
                {
                    int menuItemId = Convert.ToInt32(row.Cells["MenuItemId"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);
                    sql = "INSERT INTO OrderItems (OrderId, MenuItemId, Quantity, Price, Amount) VALUES (@OrderId, @MenuItemId, @Quantity, @Price, @Amount)";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Order saved successfully!");
                ClearOrder();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}