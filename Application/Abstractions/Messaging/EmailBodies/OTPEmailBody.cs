using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.EmailBodies
{
    public class OTPEmailBody
    {
        public static string OtpCheckingMail(string baseUrl, string email, int otp, string component, Guid questionId)
        {
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
                        <h1>The Device Check OTP :</h1>
                        <h2 style=""
                            display: flex;
                            justify-content: center; /* horizontal */
                            align-items: center;     /* vertical */
                            height: 100vh;           /* full height example */
                        "">
                        { otp}
                        </h2>
                        < hr>
                        <p>
                            If you did not request this code, please ignore this email, if you orderd to access your account from a new device, please use the code above to verify your identity within 2 minutes.
                        </p>
                        <br>
                        <a class=""button"" href=""{baseUrl}/{component}?email={email}&code={questionId}"">Go To the Website</a>
                    </ body >
                </ html >
            ";
        }
    }
}
