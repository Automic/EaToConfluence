using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaAddIn
{
    public class CredentialService
    {
        string username = "";
        string password = "";

        public Tuple<string, string> GetCredentials()
        {
           if(string.IsNullOrEmpty(username))
           {
               SetCredentials();
           }

           return new Tuple<string, string>(username, password);
        }

        public void SetCredentials()
        {
            var form = new Credentials();

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // TODO: prevent form closing if no credentials are filled in
                username = form.Username.Text;
                password = form.Password.Text;
            }
        }
    }
}
