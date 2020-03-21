using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Library
{
    public interface IMessage
    {
        void LongAlert(string message);

        void ShortAlert(string message);
    }
}