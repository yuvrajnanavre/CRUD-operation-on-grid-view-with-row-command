using System.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Senwell_Crud_Operation
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private SqlDataAdapter SqlDA;
        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {

        }
        void BindGrid()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_select", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDA = new SqlDataAdapter(cmd);
                    DataSet dt = new DataSet();
                    SqlDA.Fill(dt);
                    cmd.Dispose();
                    con.Close();
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error!');", true);
            }
        }
        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string Amount = (row.FindControl("txtamount") as TextBox).Text;
            string date = (row.FindControl("txtdate") as TextBox).Text.Trim();
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr)) 
            {
                SqlCommand cmd = new SqlCommand("sp_update1", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Amount", Amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@ModifiedBy", Convert.ToString(Session["UserName"]));
                cmd.Connection = con;
                con.Open();
                SqlDA = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                SqlDA.Fill(dt);
                cmd.Dispose();
                con.Close();
                BindGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('" + dt.Tables[0].Rows[0][0].ToString() + "');", true);
                GridView1.EditIndex = -1;
                BindGrid();
            }
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Connection = con;
                    con.Open();
                    SqlDA = new SqlDataAdapter(cmd);
                    DataSet dt = new DataSet();
                    SqlDA.Fill(dt);
                    cmd.Dispose();
                    con.Close();
                    BindGrid();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('" + dt.Tables[0].Rows[0][0].ToString() + "');", true);

                }
            }
            this.BindGrid();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            this.BindGrid();

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}