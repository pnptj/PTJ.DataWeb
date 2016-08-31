using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for WebBase
/// </summary>
public class WebBase : System.Web.UI.Page
{
    protected dbDataContext ctx = null;
    protected string fromTable = "";
    protected string toTable = "";
    protected Dictionary<int, string> dicFromTableCol = null;
    protected Dictionary<int, string> dicFromTableColValue = null;
    protected Dictionary<int, string> dicToTableCol = null;
    protected Dictionary<int, string> dicToTableColValue = null;

	public WebBase()
	{
        ctx = new dbDataContext();
	}

    protected virtual void getViewStates() 
    {
        if (rqFromTable != "") { vsFromTable = rqFromTable; fromTable = vsFromTable; } else { fromTable = vsFromTable; }
        if (rqToTable != "") { vsToTable = rqToTable; toTable = vsToTable; } else { toTable = vsToTable; }
        if (rqFromTableCol != null) { vsFromTableCol = rqFromTableCol; dicFromTableCol = vsFromTableCol;} else { dicFromTableCol = vsFromTableCol; }
        if (rqFromTableColValue != null) { vsFromTableColValue = rqFromTableColValue; dicFromTableColValue = vsFromTableColValue; rqFromTableColValue = null; } else { dicFromTableColValue = vsFromTableColValue; }
        if (rqToTableCol != null) { vsToTableCol = rqToTableCol; dicToTableCol = vsToTableCol; } else { dicToTableCol = vsToTableCol; }
        if (rqToTableColValue != null) { vsToTableColValue = rqToTableColValue; dicToTableColValue = vsToTableColValue; rqToTableColValue = null; } else { dicToTableColValue = vsToTableColValue; }
    }

    protected void clearViewState() { ViewState.Clear(); }
    protected bool isViewStateFromEmpty { get { return vsFromTable == "" && vsFromTableCol == null && vsFromTableColValue == null; } }
    protected bool isViewStateToEmpty { get { return vsToTable == "" && vsToTable == "" && vsToTableCol == null && vsToTableColValue == null; } }
    protected bool isViewStateEmpty { get { return isViewStateFromEmpty && isViewStateToEmpty; } }

    protected virtual void setViewStates()
    {
        vsFromTable = fromTable;
        vsToTable = toTable;
        vsFromTableCol = dicFromTableCol;
        vsFromTableColValue = dicFromTableColValue;
        vsToTableCol = dicToTableCol;
        vsToTableColValue = dicToTableColValue;
    }

    protected void clearRequestStates()
    {
        PropertyInfo Isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        Isreadonly.SetValue(Request.QueryString, false, null);
        Request.QueryString.Clear();
    }

    protected Dictionary<int, string> getValue(GridView gridView, Dictionary<int, string> dicFromTableCol)
    {
        Dictionary<int, string> dicResult = new Dictionary<int, string>();
        dicFromTableCol.ToList().ForEach(f => dicResult.Add(f.Key, getValue(gridView,f.Key)));
        return dicResult;
    }

    protected string getValue(GridView gridView, int colIndexValue)
    {
        return gridView.SelectedRow.Cells[Convert.ToInt32(colIndexValue) + 1].Text.ToString();
    }

    protected List<foreignKey> getFK(dbDataContext ctx, string tableNameFrom, string tableNameTo)
    {
        string sql = "SELECT ";
        sql = sql + "F_Table = FK.TABLE_NAME, ";
        sql = sql + "FK_Column = CU.COLUMN_NAME, ";
        sql = sql + "PK_Table = PK.TABLE_NAME, ";
        sql = sql + "PK_Column = PT.COLUMN_NAME, ";
        sql = sql + "Constraint_Name = C.CONSTRAINT_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN ( ";
        sql = sql + "SELECT i1.TABLE_NAME, i2.COLUMN_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME ";
        sql = sql + "WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' ";
        sql = sql + ") PT ON PT.TABLE_NAME = PK.TABLE_NAME ";
        sql = sql + "AND FK.TABLE_NAME = {1} ";
        sql = sql + "AND  PK.TABLE_NAME = {0} ";

        return ctx.ExecuteQuery<foreignKey>(sql, new string[] { tableNameFrom, tableNameTo }).ToList();
    }

    protected List<foreignKey> getFKVariants(dbDataContext ctx, string tableNameFrom, string tableNameTo)
    {
        string sql = "SELECT ";
        sql = sql + "F_Table = FK.TABLE_NAME, ";
        sql = sql + "FK_Column = CU.COLUMN_NAME, ";
        sql = sql + "PK_Table = PK.TABLE_NAME, ";
        sql = sql + "PK_Column = PT.COLUMN_NAME, ";
        sql = sql + "Constraint_Name = C.CONSTRAINT_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME ";
        sql = sql + "INNER JOIN ( ";
        sql = sql + "SELECT i1.TABLE_NAME, i2.COLUMN_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 ";
        sql = sql + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME ";
        sql = sql + "WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' ";
        sql = sql + ") PT ON PT.TABLE_NAME = PK.TABLE_NAME ";
        sql = sql + "AND ((FK.TABLE_NAME = {0} OR FK.TABLE_NAME = {1}) ";
        sql = sql + "OR  (PK.TABLE_NAME = {0} OR PK.TABLE_NAME = {1})) ";

        return ctx.ExecuteQuery<foreignKey>(sql, new string[] { tableNameFrom, tableNameTo }).ToList();
    }

    protected DataTable Cast<T>(IEnumerable<T> varlist, string tableName)
    {
        DataTable dtReturn = new DataTable() { TableName = tableName };

        // column names 
        PropertyInfo[] oProps = null;

        if (varlist == null) return dtReturn;

        foreach (T rec in varlist.Take(1))
        {
            // Use reflection to get property names, to create table, Only first time, others will follow 
            if (oProps == null)
            {
                oProps = ((Type)rec.GetType()).GetProperties();
                foreach (PropertyInfo pi in oProps)
                {
                    Type colType = pi.PropertyType;

                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                    == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }

                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                }
            }

            DataRow dr = dtReturn.NewRow();

            foreach (PropertyInfo pi in oProps)
            {
                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                (rec, null);
            }

            dtReturn.Rows.Add(dr);
        }
        return dtReturn;
    }

    protected Dictionary<int, string> getPrimaryKey(DataTable table)
    {
        string sql = "SELECT COLUMN_NAME ";
        sql = sql + "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ";
        sql = sql + "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 ";
        sql = sql + "AND TABLE_NAME = {0}";
        List<string> lstPrimColNames = ctx.ExecuteQuery<string>(sql, new object[] { table.TableName }).ToList();
        Dictionary<int, string> dicPrimCols = table.Columns.Cast<DataColumn>().Where(w => lstPrimColNames.Contains(w.ColumnName)).Select(s => new { Key = s.Ordinal, Value = s.ColumnName }).ToDictionary(d => d.Key, d => d.Value);
        return dicPrimCols;
    }

    protected class foreignKey
    {
        public string f_Table;
        public string F_Table { get { return this.f_Table; } set { this.f_Table = value; } }
        public string fK_Column;
        public string FK_Column { get { return this.fK_Column; } set { this.fK_Column = value; } }
        public string pK_Table;
        public string PK_Table { get { return this.pK_Table; } set { this.pK_Table = value; } }
        public string pK_Column;
        public string PK_Column { get { return this.pK_Column; } set { this.pK_Column = value; } }
        public string constraint_Name;
        public string Constraint_Name { get { return this.constraint_Name; } set { this.constraint_Name = value; } }
    }

    protected string vsFromTable { get { if (ViewState["fromTable"] != null) { return ViewState["fromTable"].ToString(); } else { return ""; } } set { ViewState["fromTable"] = value; } }
    protected string vsToTable { get { if (ViewState["toTable"] != null) { return ViewState["toTable"].ToString(); } else { return ""; } } set { ViewState["toTable"] = value; } }

    protected Dictionary<int, string> vsFromTableCol
    {
        get
        {
            if (ViewState["dicFromTableCol"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in ViewState["dicFromTableCol"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                ViewState["dicFromTableCol"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> vsFromTableColValue
    {
        get
        {
            if (ViewState["dicFromTableColValue"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in ViewState["dicFromTableColValue"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                ViewState["dicFromTableColValue"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> vsToTableCol
    {
        get
        {
            if (ViewState["dicToTableCol"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in ViewState["dicToTableCol"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                ViewState["dicToTableCol"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> vsToTableColValue
    {
        get
        {
            if (ViewState["dicToTableColValue"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in ViewState["dicToTableColValue"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                ViewState["dicToTableColValue"] = tmpString;
            }
        }
    }

    protected string strVsFromTableCol
    {
        get
        {
            if (ViewState["dicFromTableCol"] != null)
            {
                return ViewState["dicFromTableCol"].ToString();
            }
            else
            {
                return null;
            }
        }
        set
        {
            ViewState["dicFromTableCol"] = value;
        }
    }
    protected string strVsFromTableColValue
    {
        get
        {
            if (ViewState["dicFromTableColValue"] != null)
            {
                return ViewState["dicFromTableColValue"].ToString();
            }
            else
            {
                return null;
            }
        }
        set
        {
            ViewState["dicFromTableColValue"] = value;
        }
    }
    protected string strVsToTableCol
    {
        get
        {
            if (ViewState["dicToTableCol"] != null)
            {
                return ViewState["dicToTableCol"].ToString();
            }
            else
            {
                return null;
            }
        }
        set
        {
            ViewState["dicToTableCol"] = value;
        }
    }
    protected string strVsToTableColValue
    {
        get
        {
            if (ViewState["dicToTableColValue"] != null)
            {
                return ViewState["dicToTableColValue"].ToString();
            }
            else
            {
                return null;
            }
        }
        set
        {
            ViewState["dicToTableColValue"] = value;
        }
    }

    protected string rqFromTable { get { if (Request.QueryString["fromTable"] != null) { return Request.QueryString["fromTable"].ToString(); } else { return ""; } } }
    protected string rqToTable { get { if (Request.QueryString["toTable"] != null) { return Request.QueryString["toTable"].ToString(); } else { return ""; } } }
    protected Dictionary<int, string> rqFromTableCol
    {
        get
        {
            if (Request.QueryString["dicFromTableCol"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in Request.QueryString["dicFromTableCol"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                Request.QueryString["dicFromTableCol"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> rqFromTableColValue
    {
        get
        {
            if (Request.QueryString["dicFromTableColValue"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in Request.QueryString["dicFromTableColValue"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                Request.QueryString["dicFromTableColValue"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> rqToTableCol
    {
        get
        {
            if (Request.QueryString["dicToTableCol"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in Request.QueryString["dicToTableCol"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                Request.QueryString["dicToTableCol"] = tmpString;
            }
        }
    }
    protected Dictionary<int, string> rqToTableColValue
    {
        get
        {
            if (Request.QueryString["dicToTableColValue"] != null)
            {
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                foreach (var item in Request.QueryString["dicToTableColValue"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dicResult.Add(Convert.ToInt32(item.Split(new string[] { ":" }, StringSplitOptions.None)[0]), item.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                }
                return dicResult;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null)
            {
                string tmpString = "";
                value.ToList().ForEach(f => tmpString = tmpString + f.Key + ":" + f.Value + ";");
                Request.QueryString["dicToTableColValue"] = tmpString;
            }
        }
    }

}