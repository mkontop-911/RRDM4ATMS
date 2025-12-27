using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLHelper
{
        //public static string ToXml(this DataTable table, int metaIndex = 0)
        //{
        //    XDocument xdoc = new XDocument(
        //        new XElement(table.TableName,
        //            from column in table.Columns.Cast<DataColumn>()
        //            where column != table.Columns[metaIndex]
        //            select new XElement(column.ColumnName,
        //                from row in table.AsEnumerable()
        //                select new XElement(row.Field<string>(metaIndex), row[column])
        //                )
        //            )
        //        );

        //    return xdoc.ToString();
        //}
 
}

