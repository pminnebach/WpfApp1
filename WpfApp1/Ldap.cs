using System.Diagnostics;
using System.DirectoryServices;

namespace WpfApp1
{
    // https://www.codemag.com/article/1312041/Using-Active-Directory-in-.NET
    public class Ldap
    {
        private static string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        public static DirectorySearcher BuildUserSearcher(DirectoryEntry de)
        {
            DirectorySearcher ds = null;

            ds = new DirectorySearcher(de);

            // Full Name
            ds.PropertiesToLoad.Add("name");

            // Email Address
            ds.PropertiesToLoad.Add("mail");

            // First Name
            ds.PropertiesToLoad.Add("givenname");

            // Last Name (Surname)
            ds.PropertiesToLoad.Add("sn");

            // Login Name
            ds.PropertiesToLoad.Add("userPrincipalName");

            // Distinguished Name
            ds.PropertiesToLoad.Add("distinguishedName");

            return ds;
        }

        public static SearchResultCollection SearchForUsers(string name)
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            // Build User Searcher
            DirectorySearcher ds = BuildUserSearcher(de);
            ds.Filter = $"(&(objectCategory=User)(objectClass=person)(name={name}*))";

            return ds.FindAll();
        }

        public static SearchResult GetAUser(string samAccountName)
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            // Build User Searcher
            DirectorySearcher ds = BuildUserSearcher(de);
            // Set the filter to look for a specific user
            ds.Filter = $"(&(objectCategory=User)(objectClass=person)(samAccountName={samAccountName})";

            //return sr = ds.FindOne();
            return ds.FindOne();

            //if (sr != null)
            //{
            //    Debug.WriteLine(sr.GetPropertyValue("name"));
            //    Debug.WriteLine(sr.GetPropertyValue("mail"));
            //    Debug.WriteLine(sr.GetPropertyValue("givenname"));
            //    Debug.WriteLine(sr.GetPropertyValue("sn"));
            //    Debug.WriteLine(sr.GetPropertyValue("userPrincipalName"));
            //    Debug.WriteLine(sr.GetPropertyValue("distinguishedName"));
            //}
        }

        public static void GetAllGroups()
        {
            SearchResultCollection results;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            DirectorySearcher ds = new DirectorySearcher(de);
            // Sort by name
            ds.Sort = new SortOption("name", SortDirection.Ascending);
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("memberof");
            ds.PropertiesToLoad.Add("member");

            ds.Filter = "(&(objectCategory=Group))";

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Debug.WriteLine(sr.Properties["name"][0].ToString());

                if (sr.Properties["memberof"].Count > 0)
                {
                    Debug.WriteLine("  Member of...");
                    foreach (string item in sr.Properties["memberof"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["member"].Count > 0)
                {
                    Debug.WriteLine("  Members");
                    foreach (string item in sr.Properties["member"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
            }
        }

        public static SearchResultCollection GetAllNestedGroups(string distinguishedName)
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            // Build User Searcher
            DirectorySearcher ds = BuildUserSearcher(de);
            ds.Filter = $"(member:1.2.840.113556.1.4.1941:={distinguishedName})";
            ds.PropertiesToLoad.Add("name");

            return ds.FindAll();
        }
    }

    public static class ADExtensionMethods
    {
        public static string GetPropertyValue(this SearchResult sr, string propertyName)
        {
            string ret = string.Empty;

            if (sr.Properties[propertyName].Count > 0)
                ret = sr.Properties[propertyName][0].ToString();

            return ret;
        }
    }
}
