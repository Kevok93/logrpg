using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using PrintF;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace logrpg.UserAuth {
    public class DataHandle {
        private MySqlConnection DBH;
        private bool auth;

        private static System.Collections.Generic.Dictionary<string,DataHandle> DH_cache =
            new System.Collections.Generic.Dictionary<string,DataHandle>();

        public static DataHandle getHandle(string connect_string) {
            if (DH_cache.ContainsKey (connect_string)) return DH_cache[connect_string];

            DataHandle self = new DataHandle();
            self.auth = false;

            self.DBH = new MySqlConnection (connect_string);
            self.DBH.Open();
            self.sql(SPrintF.sprintf ("set @auth='%s';","unset"));
            Log.Debug ("Dataserver connection sucessful");
            //do other stuff maybe?

            return self;
        }

        public List<Dictionary<String,Object>> sql(string query) {
            var result_set = new List<Dictionary<String, Object>>();

            Log.FormatTrace("Executing query '%s'", query);
            MySqlCommand stmt = new MySqlCommand(query, DBH);
            MySqlDataReader results = stmt.ExecuteReader();
            if (results.GetSchemaTable() == null) {
                results.Close();
                return result_set;
            }
            var schema = results.GetSchemaTable().Select();

            while (results.Read()) {
                Dictionary<String, Object> row = new Dictionary<String, Object>();

                foreach (int i in Enumerable.Range (0, schema.Length)) {
                    string col = Convert.ToString(schema[i].Field<String>("ColumnName"));
                    row[col] = results[i];
                }
                result_set.Add (row);
            }
            Log.FormatTrace("%d rows returned", result_set.Count);
            return result_set;
        }

        public void Auth(
            string user,
            string pass
        ) {
            string passhash = SHA512String (user + pass);

            sql (SPrintF.sprintf ("set @auth='%s';", passhash));

            if (checkAuth() == false) {
                throw new AuthenticationException (SPrintF.sprintf ("Unable to authenticate user %s.", user));
            } else {
                Log.FormatWarn ("Sucessfully Authenticated as %s!", user);
            }
        }

        public bool checkAuth() {
            int keylength = Convert.ToInt32 (new MySqlCommand("select length(@auth) as keylength", DBH).ExecuteScalar());
            this.auth = keylength == 128;
            return this.auth;
        }

        public static string SHA512String(string input) {
            Byte[] in_bytes  = Encoding.UTF8.GetBytes (input);
            Byte[] out_bytes = SHA512.Create().ComputeHash (in_bytes);
            string output = out_bytes
                .Select (b => b.ToString ("x2"))
                .Aggregate ((word,b) => word += b);
            return output;
        }
    }
}