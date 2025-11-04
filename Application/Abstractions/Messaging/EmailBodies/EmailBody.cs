using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.EmailBodies
{
    public class EmailBody
    {
        public static string RedirectionMail(string baseUrl ,string email, string emailMessage, string token, string component, string shortMessage)
        {
            string encodeToken = Uri.EscapeDataString(token);
            return
            $@"
                <html>
                    <head>
                        <style>
                            .button{{
                                border: none;
                                border-redius: 1-px;
                                padding: 15px 30px;
                                color: #fff;
                                display: inline-block;
                                cursor: pointer;
                                text-decoration: none;
                                box-shadow: 0 4px 15px rgba(0,0,0,0.2);
                                transition: all .3s ease;
                                font-size: 16px;
                                font-weight: bold;
                                font-family: 'Arial', sans-serif;
                                animation: glow 1.5s infinite alternate;
                                background: linear-gradient(45deg, #ff7e5f, #feb47b);
                             }}
                        </style>
                    </head>
                    <body>
                        <h1>{emailMessage}</h1>
                        <hr>
                        <br>
                        <a class=""button"" href=""{baseUrl}/{component}?email={email}&code={encodeToken}"">{shortMessage}</a>
                    </ body >
                </ html >
            ";
        }
    }
}
