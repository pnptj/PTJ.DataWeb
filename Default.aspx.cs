using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : WebBase
{
    public _Default()
    {
        

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        getViewStates();

        if (!IsPostBack)
        {
            if (isViewStateEmpty)
            {
                List<string> lstTableNames = ctx.ExecuteQuery<string>("SELECT Name FROM Sys.tables WHERE name <> {0} ORDER BY Name", new object[] { "sysdiagrams" }).ToList<string>();
                lstTableNames.Insert(0, "Not Assigned");
                ddlTables.DataSource = lstTableNames;
                ddlTables.DataBind();
            }
            if (!isViewStateFromEmpty & !isViewStateToEmpty)
            {
                updateGridViewRelationalData();
            }
        }
        else
        {

        }

        setViewStates();
    }

    private void updateGridViewRelationalData()
    {
        switch (toTable)
        {
            case "Adress":
                gvDataResults.DataSource = ctx.ExecuteQuery<Adress>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Adress>(ctx.Adresses.Take(1), "Adress"));
               break;
            case "AdressTyp":
                gvDataResults.DataSource = ctx.ExecuteQuery<AdressTyp>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<AdressTyp>(ctx.AdressTyps.Take(1), "AdressTyp"));
                break;
            case "AdressVariant":
                gvDataResults.DataSource = ctx.ExecuteQuery<AdressVariant>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<AdressVariant>(ctx.AdressVariants.Take(1), "AdressVariant"));
                break;
            case "GatuAdress":
                gvDataResults.DataSource = ctx.ExecuteQuery<GatuAdress>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<GatuAdress>(ctx.GatuAdresses.Take(1), "GatuAdress"));
                break;
            case "Mail":
                gvDataResults.DataSource = ctx.ExecuteQuery<Mail>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Mail>(ctx.Mails.Take(1), "Mail"));
                break;
            case "Organisation":
                gvDataResults.DataSource = ctx.ExecuteQuery<Organisation>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Organisation>(ctx.Organisations.Take(1), "Organisation"));
                break;
            case "Organisation_Adress":
                gvDataResults.DataSource = ctx.ExecuteQuery<Organisation_Adress>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Organisation_Adress>(ctx.Organisation_Adresses.Take(1), "Organisation_Adress"));
                break;
            case "Person":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person>(ctx.Persons.Take(1), "Person"));
                break;
            case "Person_Adress":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_Adress>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_Adress>(ctx.Person_Adresses.Take(1), "Person_Adress"));
                break;
            case "Person_AnnanPerson":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_AnnanPerson>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_AnnanPerson>(ctx.Person_AnnanPersons.Take(1), "Person_AnnanPerson"));
                break;
            case "Person_Anställd":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_Anställd>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_Anställd>(ctx.Person_Anställds.Take(1), "Person_Anställd"));
                break;
            case "Person_Konsult":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_Konsult>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_Konsult>(ctx.Person_Konsults.Take(1), "Person_Konsult"));
                break;
            case "Person_Patient":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_Patient>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_Patient>(ctx.Person_Patients.Take(1), "Person_Patient"));
                break;
            case "Person_Sjuk_Hälsovårds_Personal":
                gvDataResults.DataSource = ctx.ExecuteQuery<Person_Sjuk_Hälsovårds_Personal>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Person_Sjuk_Hälsovårds_Personal>(ctx.Person_Sjuk_Hälsovårds_Personals.Take(1), "Person_Sjuk_Hälsovårds_Personal"));
                break;
            case "Telefon":
                gvDataResults.DataSource = ctx.ExecuteQuery<Telefon>(getSQLRelations(), new object[] { });
                setSwapViewStates();
                dicFromTableCol = getPrimaryKey(Cast<Telefon>(ctx.Telefons.Take(1), "Telefon"));
                break;
        }
        gvDataResults.DataBind();
    }

    private void setSwapViewStates()
    {
        fromTable = toTable;
        toTable = "";
        dicFromTableCol = null;
        dicFromTableColValue = null;
        dicToTableCol = null;
        dicToTableColValue = null;
        clearRequestStates();
        setViewStates();
     }
    
    private string getSQLRelations()
    {
        string sql = "";
        List<foreignKey> lstFKs = getFK(ctx, fromTable, toTable);

        if (lstFKs.Count() == 0) { lstFKs = getFK(ctx, toTable, fromTable); }
        foreignKeyValues lstFkValues = getForeignKeysVariables(lstFKs).First();

        if (lstFkValues.ToTable == fromTable)
        {
            string primaryColumnName = getPrimaryKey(fromTable).First();

            sql = "SELECT DISTINCT a.* ";
            sql = sql + "FROM " + lstFkValues.FromTableSchema + "." + lstFkValues.FromTable + " a ";
            sql = sql + "JOIN " + lstFkValues.ToTableSchema + "." + lstFkValues.ToTable + " b ON a." + lstFkValues.FromColumn + " = b." + lstFkValues.ToColumn + " ";
            if (lstFkValues.FromValue != "")
            {
                sql = sql + "WHERE a." + lstFkValues.FromColumn + " = " + lstFkValues.FromValue;
            }
            else if (lstFkValues.ToValue != "")
            {
                sql = sql + "WHERE b." + primaryColumnName + " = " + lstFkValues.ToValue;
            }
        }
        else if (lstFkValues.ToTable == toTable)
        {
            string primaryColumnName = getPrimaryKey(toTable).First();
            
            sql = "SELECT DISTINCT b.* ";
            sql = sql + "FROM " + lstFkValues.FromTableSchema + "." + lstFkValues.FromTable + " a ";
            sql = sql + "JOIN " + lstFkValues.ToTableSchema + "." + lstFkValues.ToTable + " b ON a." + lstFkValues.FromColumn + " = b." + lstFkValues.ToColumn + " ";
            if (lstFkValues.FromValue != "")
            {
                sql = sql + "WHERE a." + primaryColumnName + " = " + lstFkValues.FromValue;
            }
            else if (lstFkValues.ToValue != "")
            {
                sql = sql + "WHERE b." + lstFkValues.ToColumn + " = " + lstFkValues.ToValue;
            }
        }

        return sql;
    }

    private List<string> getPrimaryKey(string tableName)
    {
        string sql = "SELECT COLUMN_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ";
        sql = sql + "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 ";
        sql = sql + "AND TABLE_NAME = {0}";
        List<string> lstPrimColNames = ctx.ExecuteQuery<string>(sql, new object[] { tableName }).ToList();
        return lstPrimColNames;
    }

    private List<foreignKeyValues> getForeignKeysVariables(List<foreignKey> lstFKs)
    {   
        List<foreignKeyValues> lstResult = null;
        List<foreignKey> lstFkKeys = (from a in lstFKs
                  join b in dicFromTableCol on a.PK_Column equals b.Value
                             select a).ToList();
        if (lstFkKeys.Count() == 0)
        {
            lstFkKeys = (from a in lstFKs
                      join b in dicFromTableCol on a.FK_Column equals b.Value
                      select a).ToList();
        }

        List<foreignKey> lstKeyTable = (from a in lstFkKeys
                      where a.PK_Table == fromTable
                      select a).ToList();

        if (lstKeyTable.Count() == 0)
        {
            lstKeyTable = (from a in lstFkKeys
                           where a.PK_Table == toTable
                           select a).ToList();
        }

        if (lstKeyTable.Count() == 0)
        {
            lstKeyTable = (from a in lstFkKeys
                           where a.F_Table == fromTable
                           select a).ToList();
        }

        if (lstKeyTable.Count() == 0)
        {
            lstKeyTable = (from a in lstFkKeys
                           where a.F_Table == toTable
                           select a).ToList();
        }
        if (lstKeyTable.Count() > 0)
        {
            lstResult = (from a in lstKeyTable
                         join b in dicFromTableCol on a.PK_Column equals b.Value
                         join c in dicFromTableColValue on b.Key equals c.Key
                         select new foreignKeyValues() { FromTableSchema = getSchemaName(a.F_Table), FromTable = a.F_Table, FromColumn = a.FK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.PK_Table), ToTable = a.PK_Table, ToColumn = a.PK_Column, ToValue = "", }).ToList();

            if (lstResult.Count() == 0)
            {
                lstResult = (from a in lstKeyTable
                             join b in dicFromTableCol on a.FK_Column equals b.Value
                             join c in dicFromTableColValue on b.Key equals c.Key
                             select new foreignKeyValues() { FromTableSchema = getSchemaName(a.PK_Table), FromTable = a.PK_Table, FromColumn = a.PK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.F_Table), ToTable = a.F_Table, ToColumn = a.FK_Column, ToValue = "", }).ToList();
            }
        }

        return lstResult;

        //lstResult = (from a in lstFKs
        //          join b in dicFromTableCol on a.PK_Column equals b.Value
        //          join c in dicFromTableColValue on b.Key equals c.Key
        //            where a.PK_Table == fromTable
        //             select new foreignKeyValues() { FromTableSchema = getSchemaName(a.PK_Table), FromTable = a.PK_Table, FromColumn = a.PK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.F_Table), ToTable = a.F_Table, ToColumn = a.FK_Column, ToValue = "", }).ToList();
        //if (lstResult.Count() != 0) { return lstResult; }

        //lstResult = (from a in lstFKs
        //          join b in dicFromTableCol on a.FK_Column equals b.Value
        //          join c in dicFromTableColValue on b.Key equals c.Key
        //             where a.PK_Table == toTable
        //             select new foreignKeyValues() { FromTableSchema = getSchemaName(a.PK_Table), FromTable = a.PK_Table, FromColumn = a.PK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.F_Table), ToTable = a.F_Table, ToColumn = a.FK_Column, ToValue = "", }).ToList();
        //if (lstResult.Count() != 0) { return lstResult; }

        //lstResult = (from a in lstFKs
        //             join b in dicToTableCol on a.PK_Column equals b.Value
        //             join c in dicToTableColValue on b.Key equals c.Key
        //             where a.F_Table == toTable
        //             select new foreignKeyValues() { FromTableSchema = getSchemaName(a.PK_Table), FromTable = a.PK_Table, FromColumn = a.PK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.F_Table), ToTable = a.F_Table, ToColumn = a.FK_Column, ToValue = "", }).ToList();
        //if (lstResult.Count() != 0) { return lstResult; }

        //lstResult = (from a in lstFKs
        //             join b in dicToTableCol on a.FK_Column equals b.Value
        //             join c in dicToTableColValue on b.Key equals c.Key
        //             where a.F_Table == fromTable
        //             select new foreignKeyValues() { FromTableSchema = getSchemaName(a.PK_Table), FromTable = a.PK_Table, FromColumn = a.PK_Column, FromValue = c.Value, ToTableSchema = getSchemaName(a.F_Table), ToTable = a.F_Table, ToColumn = a.FK_Column, ToValue = "", }).ToList();
        //if (lstResult.Count() != 0) { return lstResult; }

        //return null;
    }

    private class foreignKeyValues
    {
        public string FromTableSchema = "";
        public string FromTable = "";
        public string FromColumn = "";
        public string FromValue = "";
        public string ToTableSchema = "";
        public string ToTable = "";
        public string ToColumn = "";
        public string ToValue = "";
    }


    private string getSchemaName(string tableName)
    {
        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ctx.Connection.ConnectionString + "password=abcd1234;"))
        {
            conn.Open();

            DataTable allTablesSchemaTable = conn.GetSchema("Tables");
            return allTablesSchemaTable.Rows.Cast<DataRow>().Where(w => w["TABLE_NAME"].ToString() == tableName).Select(s => s["TABLE_SCHEMA"].ToString()).First();

        }
    }

    protected void ddlTables_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvDataResults.Caption = ddlTables.SelectedItem.Text;
        fromTable = gvDataResults.Caption;
        switch (ddlTables.SelectedItem.Text)
        {
            case "Not Assigned":
                gvDataResults.DataSource = null;
                break;
            case "Adress":
                gvDataResults.DataSource = ctx.Adresses;
                dicFromTableCol = getPrimaryKey(Cast<Adress>(ctx.Adresses.Take(1), "Adress"));
                break;
            case "AdressTyp":
                gvDataResults.DataSource = ctx.AdressTyps;
                dicFromTableCol = getPrimaryKey(Cast<AdressTyp>(ctx.AdressTyps.Take(1), "AdressTyp"));
                break;
            case "AdressVariant":
                gvDataResults.DataSource = ctx.AdressVariants;
                dicFromTableCol = getPrimaryKey(Cast<AdressVariant>(ctx.AdressVariants.Take(1), "AdressVariant"));
                break;
            case "GatuAdress":
                gvDataResults.DataSource = ctx.GatuAdresses;
                dicFromTableCol = getPrimaryKey(Cast<GatuAdress>(ctx.GatuAdresses.Take(1), "GatuAdress"));
                break;
            case "Mail":
                gvDataResults.DataSource = ctx.Mails;
                dicFromTableCol = getPrimaryKey(Cast<Mail>(ctx.Mails.Take(1), "Mail"));
                break;
            case "Organisation":
                gvDataResults.DataSource = ctx.Organisations;
                dicFromTableCol = getPrimaryKey(Cast<Organisation>(ctx.Organisations.Take(1), "Organisation"));
                break;
            case "Organisation_Adress":
                gvDataResults.DataSource = ctx.Organisation_Adresses;
                dicFromTableCol = getPrimaryKey(Cast<Organisation_Adress>(ctx.Organisation_Adresses.Take(1), "Organisation_Adress"));
                break;
            case "Person":
                gvDataResults.DataSource = ctx.Persons;
                dicFromTableCol = getPrimaryKey(Cast<Person>(ctx.Persons.Take(1), "Person"));
                break;
            case "Person_Adress":
                gvDataResults.DataSource = ctx.Person_Adresses;
                dicFromTableCol = getPrimaryKey(Cast<Person_Adress>(ctx.Person_Adresses.Take(1), "Person_Adress"));
                break;
            case "Person_AnnanPerson":
                gvDataResults.DataSource = ctx.Person_AnnanPersons;
                dicFromTableCol = getPrimaryKey(Cast<Person_AnnanPerson>(ctx.Person_AnnanPersons.Take(1), "Person_AnnanPerson"));
                break;
            case "Person_Anställd":
                gvDataResults.DataSource = ctx.Person_Anställds;
                dicFromTableCol = getPrimaryKey(Cast<Person_Anställd>(ctx.Person_Anställds.Take(1), "Person_Anställd"));
                break;
            case "Person_Konsult":
                gvDataResults.DataSource = ctx.Person_Konsults;
                dicFromTableCol = getPrimaryKey(Cast<Person_Konsult>(ctx.Person_Konsults.Take(1), "Person_Konsult"));
                break;
            case "Person_Patient":
                gvDataResults.DataSource = ctx.Person_Patients;
                dicFromTableCol = getPrimaryKey(Cast<Person_Patient>(ctx.Person_Patients.Take(1), "Person_Patient"));
                break;
            case "Person_Sjuk_Hälsovårds_Personal":
                gvDataResults.DataSource = ctx.Person_Sjuk_Hälsovårds_Personals;
                dicFromTableCol = getPrimaryKey(Cast<Person_Sjuk_Hälsovårds_Personal>(ctx.Person_Sjuk_Hälsovårds_Personals.Take(1), "Person_Sjuk_Hälsovårds_Personal"));
                break;
            case "Telefon":
                gvDataResults.DataSource = ctx.Telefons;
                dicFromTableCol = getPrimaryKey(Cast<Telefon>(ctx.Telefons.Take(1), "Telefon"));
                break;
        }
        setViewStates();

        gvDataResults.DataBind();
    }
    protected void gvDataResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (fromTable != "" && toTable == "")
        {
            dicFromTableColValue = getValue(gvDataResults, dicFromTableCol);
            setViewStates();

            string redirectUrl = "~/DataRelations.aspx?";
            redirectUrl = redirectUrl + "FromTable=" + vsFromTable;
            redirectUrl = redirectUrl + "&ToTable=" + vsToTable;
            redirectUrl = redirectUrl + "&dicFromTableCol=" + strVsFromTableCol;
            redirectUrl = redirectUrl + "&dicFromTableColValue=" + strVsFromTableColValue;
            redirectUrl = redirectUrl + "&dicToTableCol=" + strVsToTableCol;
            redirectUrl = redirectUrl + "&dicToTableColValue=" + strVsToTableColValue;
            Response.Redirect(redirectUrl, false);
        }
    }
    protected void btnClearNavigation_Click(object sender, EventArgs e)
    {
        clearViewState(); clearRequestStates();
        List<string> lstTableNames = ctx.ExecuteQuery<string>("SELECT Name FROM Sys.tables WHERE name <> {0} ORDER BY Name", new object[] { "sysdiagrams" }).ToList<string>();
        lstTableNames.Insert(0, "Not Assigned");
        ddlTables.DataSource = lstTableNames;
        ddlTables.DataBind();
    }
}