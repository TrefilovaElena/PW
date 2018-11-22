using System;
using System.Collections.Generic;
using System.Text;

namespace PW.ViewModels
{
    public delegate void AccountStateHandler(object sender, AccountEventArgs e);

    public class AccountEventArgs
    {
        public string Message { get; private set; }

        public AccountEventArgs(string _mes)
        {
            Message = _mes;
        }
    }
}