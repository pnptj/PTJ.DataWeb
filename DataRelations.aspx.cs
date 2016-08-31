using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DataRelations : WebBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        getViewStates();
       
        if (!IsPostBack)
        {
            txtTableName.Text = fromTable;
            gvDataResults.DataSource = getFKVariants(ctx, fromTable, fromTable);
            gvDataResults.DataBind();
        }

        setViewStates();
    }
    protected void gvDataResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (fromTable == getValue(gvDataResults, 0))
        {
            toTable = getValue(gvDataResults, 2);
        }
        else
        {
            toTable = getValue(gvDataResults, 0);
        }
      
        if (fromTable != "" && toTable != "")
        {
            setViewStates();

            string redirectUrl = "~/Default.aspx?";
            redirectUrl = redirectUrl + "FromTable=" + vsFromTable;
            redirectUrl = redirectUrl + "&ToTable=" + vsToTable;
            redirectUrl = redirectUrl + "&dicFromTableCol=" + strVsFromTableCol;
            redirectUrl = redirectUrl + "&dicFromTableColValue=" + strVsFromTableColValue;
            redirectUrl = redirectUrl + "&dicToTableCol=" + strVsToTableCol;
            redirectUrl = redirectUrl + "&dicToTableColValue=" + strVsToTableColValue;
            Response.Redirect(redirectUrl, false);
        }
    }
}