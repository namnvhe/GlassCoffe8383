using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace Cafe.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var subject = "Đặt lại mật khẩu - Cafe Manager";
            var htmlBody = EmailTemplate.GetPasswordResetEmail(resetLink);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendWelcomeEmail(string email, string fullName, string tempPassword)
        {
            var subject = "Chào mừng bạn đến với Cafe Manager - Kích hoạt tài khoản";
            var htmlBody = EmailTemplate.GetWelcomeEmail(fullName, email, tempPassword);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendFirstLoginSuccessEmail(string email, string fullName)
        {
            var subject = "Tài khoản đã được kích hoạt thành công - Cafe Manager";
            var htmlBody = EmailTemplate.GetFirstLoginSuccessEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendAccountUpdateEmail(string email, string fullName)
        {
            var subject = "Thông tin tài khoản đã được cập nhật - Cafe Manager";
            var htmlBody = EmailTemplate.GetAccountUpdateEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendAccountDeleteEmail(string email, string fullName)
        {
            var subject = "Tài khoản đã bị xóa - Cafe Manager";
            var htmlBody = EmailTemplate.GetAccountDeleteEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendAccountDeactivateEmail(string email, string fullName)
        {
            var subject = "Tài khoản đã bị tạm khóa - Cafe Manager";
            var htmlBody = EmailTemplate.GetAccountDeactivateEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendAccountActivateEmail(string email, string fullName)
        {
            var subject = "Tài khoản đã được kích hoạt lại - Cafe Manager";
            var htmlBody = EmailTemplate.GetAccountActivateEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendPasswordChangeConfirmationEmail(string email, string fullName)
        {
            var subject = "Mật khẩu đã được thay đổi thành công - Cafe Manager";
            var htmlBody = EmailTemplate.GetPasswordChangeConfirmationEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendPasswordResetSuccessEmail(string email, string fullName)
        {
            var subject = "Đặt lại mật khẩu thành công - Cafe Manager";
            var htmlBody = EmailTemplate.GetPasswordResetSuccessEmail(fullName);
            await SendEmailAsync(email, subject, htmlBody);
        }

        private async Task SendEmailAsync(string email, string subject, string htmlBody)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtpSettings["SmtpServer"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpSettings["SenderEmail"], smtpSettings["SenderPassword"]);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }

    public static class EmailTemplate
    {
        private static string GetBaseTemplate(string title, string content, string buttonHtml = "")
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title} - Cafe Manager</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f8f9fa;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #6b4423 0%, #8b5a3c 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            font-size: 28px;
            margin-bottom: 10px;
            font-weight: 600;
        }}
        .header p {{
            font-size: 16px;
            opacity: 0.9;
        }}
        .coffee-icon {{
            font-size: 48px;
            margin-bottom: 15px;
        }}
        .content {{
            padding: 40px 30px;
            text-align: center;
        }}
        .message {{
            font-size: 16px;
            color: #555;
            margin-bottom: 30px;
            line-height: 1.8;
            text-align: left;
        }}
        .button {{
            display: inline-block;
            background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
            color: white;
            padding: 15px 35px;
            text-decoration: none;
            border-radius: 50px;
            font-weight: 600;
            font-size: 16px;
            margin: 20px 0;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(230, 126, 34, 0.3);
        }}
        .button:hover {{
            background: linear-gradient(135deg, #d35400 0%, #e67e22 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(230, 126, 34, 0.4);
        }}
        .info-box {{
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
            margin: 30px 0;
            text-align: left;
        }}
        .info-box h3 {{
            color: #6b4423;
            margin-bottom: 15px;
            font-size: 18px;
        }}
        .info-box p {{
            margin-bottom: 10px;
            color: #555;
        }}
        .warning-box {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 10px;
            padding: 20px;
            margin: 30px 0;
            color: #856404;
            text-align: left;
        }}
        .success-box {{
            background-color: #e8f5e9;
            border: 1px solid #c8e6c9;
            border-radius: 10px;
            padding: 20px;
            margin: 30px 0;
            color: #2e7d32;
            text-align: left;
        }}
        .danger-box {{
            background-color: #ffebee;
            border: 1px solid #ffcdd2;
            border-radius: 10px;
            padding: 20px;
            margin: 30px 0;
            color: #c62828;
            text-align: left;
        }}
        .footer {{
            background-color: #6b4423;
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .footer p {{
            margin-bottom: 10px;
        }}
        .contact-info {{
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid #8b5a3c;
        }}
        .contact-info p {{
            font-size: 14px;
            margin: 5px 0;
        }}
        .divider {{
            height: 1px;
            background: linear-gradient(to right, transparent, #ddd, transparent);
            margin: 30px 0;
        }}
        @media only screen and (max-width: 600px) {{
            .container {{
                margin: 0;
                border-radius: 0;
            }}
            .content {{
                padding: 30px 20px;
            }}
            .header h1 {{
                font-size: 24px;
            }}
            .message {{
                font-size: 14px;
            }}
            .button {{
                padding: 15px 25px;
                font-size: 14px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""coffee-icon"">☕</div>
            <h1>Cafe Manager</h1>
            <p>{title}</p>
        </div>
        
        <div class=""content"">
            {content}
            {buttonHtml}
            <div class=""divider""></div>
        </div>

        <div class=""footer"">
            <p><strong>Cafe Manager</strong> - Hệ thống quản lý quán cà phê</p>
            <div class=""contact-info"">
                <p>📧 Email hỗ trợ: support@cafemanager.com</p>
                <p>📞 Hotline: 1900-CAFE (2233)</p>
                <p>🕐 Thời gian hỗ trợ: 8:00 - 22:00 hàng ngày</p>
            </div>
            <p style=""margin-top: 20px; font-size: 12px; opacity: 0.8;"">
                © 2024 Cafe Manager. Tất cả quyền được bảo lưu.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GetPasswordResetEmail(string resetLink)
        {
            var content = $@"
                <div class=""message"">
                    <h2 style=""color: #6b4423; margin-bottom: 20px; text-align: center;"">Đặt lại mật khẩu</h2>
                    <p style=""text-align: center;"">Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                    <p style=""text-align: center;"">Nhấn vào nút bên dưới để tạo mật khẩu mới:</p>
                </div>
                
                <div class=""warning-box"">
                    <h3>⏰ Thời hạn sử dụng</h3>
                    <p>Link này chỉ có hiệu lực trong <strong>1 giờ</strong> kể từ khi email này được gửi.</p>
                    <p>Vì lý do bảo mật, link sẽ tự động hết hạn sau thời gian trên.</p>
                </div>";

            var buttonHtml = $@"<a href=""{resetLink}"" class=""button"">🔑 Đặt lại mật khẩu</a>";

            return GetBaseTemplate("Đặt lại mật khẩu", content, buttonHtml);
        }

        public static string GetWelcomeEmail(string fullName, string email, string tempPassword)
        {
            var content = $@"
            <div class=""message"">
                <h2 style=""color: #6b4423; margin-bottom: 20px; text-align: center;"">Chào mừng {fullName}!</h2>
                <p style=""text-align: center;"">Tài khoản của bạn đã được tạo thành công tại <strong>Cafe Manager</strong>.</p>
            </div>

            <div class=""info-box"">
                <h3>📋 Thông tin đăng nhập</h3>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Mật khẩu tạm thời:</strong> {tempPassword}</p>
            </div>

            <div class=""danger-box"">
                <h3>⚠️ QUAN TRỌNG - Kích hoạt tài khoản</h3>
                <p><strong>Bạn có 7 ngày để đăng nhập lần đầu tiên!</strong></p>
                <p>• Tài khoản hiện đang ở trạng thái <strong>chờ kích hoạt</strong></p>
                <p>• Sau 7 ngày nếu chưa đăng nhập, tài khoản sẽ bị <strong>xóa tự động</strong></p>
                <p>• Đăng nhập lần đầu sẽ tự động kích hoạt tài khoản của bạn</p>
            </div>

            <div class=""warning-box"">
                <h3>🔐 Bảo mật tài khoản</h3>
                <p>• Sau khi đăng nhập lần đầu, hãy <strong>đổi mật khẩu</strong> ngay lập tức</p>
                <p>• Không chia sẻ thông tin đăng nhập với bất kỳ ai</p>
                <p>• Sử dụng mật khẩu mạnh với ít nhất 8 ký tự</p>
            </div>

            <div class=""success-box"">
                <h3>🎉 Bước tiếp theo</h3>
                <p>1. <strong>Nhấn nút ""Đăng nhập ngay"" bên dưới</strong></p>
                <p>2. Sử dụng email và mật khẩu tạm thời để đăng nhập</p>
                <p>3. Tài khoản sẽ được kích hoạt tự động</p>
                <p>4. Thay đổi mật khẩu của bạn</p>
                <p>5. Khám phá các tính năng tuyệt vời của Cafe Manager!</p>
            </div>";

            var loginUrl = $"http://localhost:3000/login";
            var buttonHtml = $@"<a href=""{loginUrl}"" class=""button"">🚀 Đăng nhập ngay để kích hoạt</a>";

            return GetBaseTemplate("Kích hoạt tài khoản trong 7 ngày", content, buttonHtml);
        }

        public static string GetFirstLoginSuccessEmail(string fullName)
        {
            var content = $@"
            <div class=""message"">
                <h2 style=""color: #6b4423; margin-bottom: 20px; text-align: center;"">Chúc mừng {fullName}!</h2>
                <p style=""text-align: center;"">Tài khoản của bạn đã được <strong>kích hoạt thành công</strong>.</p>
            </div>

            <div class=""success-box"">
                <h3>✅ Tài khoản đã sẵn sàng</h3>
                <p>• Tài khoản đã được kích hoạt và xác thực email tự động</p>
                <p>• Bạn có thể sử dụng đầy đủ các tính năng của hệ thống</p>
                <p>• Thông tin đăng nhập đã được lưu an toàn</p>
            </div>

            <div class=""warning-box"">
                <h3>🔐 Khuyến nghị bảo mật</h3>
                <p>• <strong>Đổi mật khẩu ngay</strong> để đảm bảo an toàn tài khoản</p>
                <p>• Sử dụng mật khẩu mạnh và duy nhất</p>
                <p>• Không chia sẻ thông tin đăng nhập</p>
            </div>

            <div class=""info-box"">
                <h3>🎯 Bắt đầu sử dụng</h3>
                <p>• Khám phá menu đa dạng của chúng tôi</p>
                <p>• Đặt món yêu thích một cách dễ dàng</p>
                <p>• Theo dõi lịch sử đơn hàng</p>
                <p>• Nhận thông báo về ưu đãi đặc biệt</p>
            </div>";

            var dashboardUrl = $"http://localhost:3000/dashboard";
            var buttonHtml = $@"<a href=""{dashboardUrl}"" class=""button"">🏠 Vào trang chủ</a>";

            return GetBaseTemplate("Tài khoản đã được kích hoạt", content, buttonHtml);
        }

        public static string GetAccountUpdateEmail(string fullName)
        {
            var content = $@"
                <div class=""message"">
                    <h2 style=""color: #6b4423; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}!</h2>
                    <p style=""text-align: center;"">Thông tin tài khoản của bạn đã được cập nhật thành công.</p>
                </div>

                <div class=""success-box"">
                    <h3>✅ Cập nhật thành công</h3>
                    <p>Các thông tin sau đã được cập nhật:</p>
                    <p>• Thông tin cá nhân (Họ tên, Email, Số điện thoại)</p>
                    <p>• Quyền truy cập (Role)</p>
                    <p>• Trạng thái tài khoản</p>
                </div>

                <div class=""info-box"">
                    <h3>🔒 Lưu ý bảo mật</h3>
                    <p>• Nếu bạn không yêu cầu thay đổi này, vui lòng liên hệ với quản trị viên ngay lập tức</p>
                    <p>• Kiểm tra lại thông tin tài khoản của bạn tại trang cá nhân</p>
                    <p>• Đảm bảo mật khẩu của bạn vẫn an toàn</p>
                </div>";

            var dashboardUrl = $"http://localhost:3000/dashboard";
            var buttonHtml = $@"<a href=""{dashboardUrl}"" class=""button"">📊 Xem tài khoản</a>";

            return GetBaseTemplate("Thông tin tài khoản đã được cập nhật", content, buttonHtml);
        }

        public static string GetAccountDeleteEmail(string fullName)
        {
            var content = $@"
                <div class=""message"">
                    <h2 style=""color: #c62828; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}</h2>
                    <p style=""text-align: center;"">Chúng tôi rất tiếc phải thông báo rằng tài khoản của bạn đã bị xóa khỏi hệ thống <strong>Cafe Manager</strong>.</p>
                </div>

                <div class=""danger-box"">
                    <h3>❌ Tài khoản đã bị xóa</h3>
                    <p>• Tài khoản của bạn đã bị xóa vĩnh viễn khỏi hệ thống</p>
                    <p>• Tất cả dữ liệu liên quan đến tài khoản đã bị xóa</p>
                    <p>• Bạn không thể đăng nhập vào hệ thống với tài khoản này nữa</p>
                    <p>• Thời gian xóa: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                </div>

                <div class=""info-box"">
                    <h3>💬 Cần hỗ trợ?</h3>
                    <p>Nếu bạn cho rằng đây là một sự nhầm lẫn hoặc cần hỗ trợ:</p>
                    <p>• Liên hệ với quản trị viên qua email hỗ trợ</p>
                    <p>• Gọi hotline để được tư vấn trực tiếp</p>
                    <p>• Cung cấp thông tin tài khoản để được hỗ trợ nhanh chất</p>
                </div>

                <div class=""success-box"">
                    <h3>🌟 Cảm ơn bạn</h3>
                    <p>Chúng tôi cảm ơn bạn đã sử dụng dịch vụ Cafe Manager.</p>
                    <p>Hy vọng trong tương lai có cơ hội được phục vụ bạn trở lại!</p>
                </div>";

            return GetBaseTemplate("Tài khoản đã bị xóa", content);
        }

        public static string GetAccountDeactivateEmail(string fullName)
        {
            var content = $@"
                <div class=""message"">
                    <h2 style=""color: #ff9800; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}</h2>
                    <p style=""text-align: center;"">Tài khoản của bạn tại <strong>Cafe Manager</strong> đã bị tạm khóa.</p>
                </div>

                <div class=""warning-box"">
                    <h3>⚠️ Tài khoản bị tạm khóa</h3>
                    <p>• Tài khoản của bạn hiện đang ở trạng thái <strong>bị khóa tạm thời</strong></p>
                    <p>• Bạn không thể đăng nhập vào hệ thống cho đến khi tài khoản được kích hoạt lại</p>
                    <p>• Thời gian khóa: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                    <p>• Tất cả dữ liệu của bạn vẫn được bảo lưu</p>
                </div>

                <div class=""info-box"">
                    <h3>🛡️ Lý do khóa tài khoản</h3>
                    <p>Tài khoản có thể bị khóa do các lý do sau:</p>
                    <p>• Vi phạm điều khoản sử dụng</p>
                    <p>• Hoạt động bất thường được phát hiện</p>
                    <p>• Yêu cầu từ quản trị viên</p>
                    <p>• Mục đích bảo mật tài khoản</p>
                </div>

                <div class=""success-box"">
                    <h3>📞 Khiếu nại và hỗ trợ</h3>
                    <p>Nếu bạn có bất kỳ thắc mắc nào về việc khóa tài khoản:</p>
                    <p>• <strong>Email hỗ trợ:</strong> support@cafemanager.com</p>
                    <p>• <strong>Hotline:</strong> 1900-CAFE (2233)</p>
                    <p>• <strong>Thời gian hỗ trợ:</strong> 8:00 - 22:00 hàng ngày</p>
                    <p></p>
                    <p><strong>Lưu ý:</strong> Nếu khiếu nại thành công, tài khoản sẽ được mở trở lại và bạn sẽ nhận được email thông báo.</p>
                </div>";

            return GetBaseTemplate("Tài khoản bị tạm khóa", content);
        }

        public static string GetAccountActivateEmail(string fullName)
        {
            var content = $@"
                <div class=""message"">
                    <h2 style=""color: #4caf50; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}!</h2>
                    <p style=""text-align: center;"">Chúng tôi vui mừng thông báo rằng tài khoản của bạn đã được <strong>kích hoạt lại</strong> thành công!</p>
                </div>

                <div class=""success-box"">
                    <h3>🎉 Tài khoản đã được kích hoạt</h3>
                    <p>• Tài khoản của bạn đã được mở trở lại</p>
                    <p>• Bạn có thể đăng nhập và sử dụng tất cả tính năng</p>
                    <p>• Thời gian kích hoạt: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                    <p>• Tất cả dữ liệu trước đó đã được khôi phục</p>
                </div>

                <div class=""info-box"">
                    <h3>🔐 Bảo mật tài khoản</h3>
                    <p>Để đảm bảo tài khoản an toàn:</p>
                    <p>• Đăng nhập và kiểm tra lại thông tin cá nhân</p>
                    <p>• Đổi mật khẩu nếu cần thiết</p>
                    <p>• Xem lại các hoạt động gần đây</p>
                    <p>• Cập nhật thông tin liên lạc</p>
                </div>

                <div class=""warning-box"">
                    <h3>⚡ Lưu ý quan trọng</h3>
                    <p>• Vui lòng tuân thủ các điều khoản sử dụng để tránh bị khóa lại</p>
                    <p>• Báo cáo ngay nếu phát hiện hoạt động bất thường</p>
                    <p>• Liên hệ hỗ trợ nếu gặp bất kỳ vấn đề nào</p>
                </div>";

            var loginUrl = $"http://localhost:3000/login";
            var buttonHtml = $@"<a href=""{loginUrl}"" class=""button"">🚀 Đăng nhập ngay</a>";

            return GetBaseTemplate("Tài khoản đã được kích hoạt lại", content, buttonHtml);
        }

        public static string GetPasswordChangeConfirmationEmail(string fullName)
        {
            var content = $@"
        <div class=""message"">
            <h2 style=""color: #4caf50; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}!</h2>
            <p style=""text-align: center;"">Mật khẩu của bạn đã được thay đổi thành công tại <strong>Cafe Manager</strong>.</p>
        </div>

        <div class=""success-box"">
            <h3>✅ Thay đổi mật khẩu thành công</h3>
            <p>• Mật khẩu của bạn đã được cập nhật</p>
            <p>• Thời gian thay đổi: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
            <p>• Tài khoản của bạn vẫn an toàn và được bảo vệ</p>
            <p>• Sử dụng mật khẩu mới để đăng nhập từ lần tiếp theo</p>
        </div>

        <div class=""info-box"">
            <h3>🔐 Bảo mật tài khoản</h3>
            <p>Để đảm bảo tài khoản luôn an toàn:</p>
            <p>• Không chia sẻ mật khẩu với bất kỳ ai</p>
            <p>• Sử dụng mật khẩu mạnh có ít nhất 8 ký tự</p>
            <p>• Kết hợp chữ hoa, chữ thường, số và ký tự đặc biệt</p>
            <p>• Thay đổi mật khẩu định kỳ để tăng cường bảo mật</p>
        </div>

        <div class=""warning-box"">
            <h3>⚠️ Lưu ý quan trọng</h3>
            <p><strong>Nếu bạn không thực hiện thay đổi này:</strong></p>
            <p>• Liên hệ ngay với bộ phận hỗ trợ</p>
            <p>• Thay đổi mật khẩu ngay lập tức</p>
            <p>• Kiểm tra các hoạt động bất thường trong tài khoản</p>
            <p>• Đảm bảo thiết bi của bạn không bị xâm nhập</p>
        </div>";

            var loginUrl = "http://localhost:3000/login";
            var buttonHtml = $@"<a href=""{loginUrl}"" class=""button"">🔑 Đăng nhập với mật khẩu mới</a>";

            return GetBaseTemplate("Mật khẩu đã được thay đổi thành công", content, buttonHtml);
        }

        public static string GetPasswordResetSuccessEmail(string fullName)
        {
            var content = $@"
        <div class=""message"">
            <h2 style=""color: #4caf50; margin-bottom: 20px; text-align: center;"">Xin chào {fullName}!</h2>
            <p style=""text-align: center;"">Mật khẩu của bạn đã được đặt lại thành công thông qua email khôi phục.</p>
        </div>

        <div class=""success-box"">
            <h3>🎉 Đặt lại mật khẩu thành công</h3>
            <p>• Mật khẩu mới đã được tạo và cập nhật</p>
            <p>• Thời gian đặt lại: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
            <p>• Link đặt lại mật khẩu đã hết hiệu lực</p>
            <p>• Bạn có thể đăng nhập với mật khẩu mới</p>
        </div>

        <div class=""info-box"">
            <h3>🔗 Quy trình hoàn tất</h3>
            <p>Bạn đã thành công thực hiện các bước sau:</p>
            <p>✓ Yêu cầu đặt lại mật khẩu</p>
            <p>✓ Nhận và sử dụng link khôi phục qua email</p>
            <p>✓ Tạo mật khẩu mới</p>
            <p>✓ Xác nhận và lưu mật khẩu</p>
        </div>

        <div class=""warning-box"">
            <h3>🛡️ Bảo mật tài khoản</h3>
            <p>Để bảo vệ tài khoản trong tương lai:</p>
            <p>• Ghi nhớ mật khẩu mới hoặc lưu trữ an toàn</p>
            <p>• Không sử dụng lại mật khẩu cũ</p>
            <p>• Kiểm tra thường xuyên hoạt động đăng nhập</p>
            <p>• Kích hoạt xác thực hai bước nếu có</p>
        </div>

        <div class=""info-box"">
            <h3>💡 Mẹo bảo mật</h3>
            <p><strong>Tạo mật khẩu mạnh:</strong></p>
            <p>• Ít nhất 8-12 ký tự</p>
            <p>• Kết hợp chữ hoa, chữ thường (A-z)</p>
            <p>• Bao gồm số (0-9)</p>
            <p>• Sử dụng ký tự đặc biệt (!@#$%^&*)</p>
            <p>• Tránh thông tin cá nhân dễ đoán</p>
        </div>";

            var loginUrl = "http://localhost:3000/login";
            var buttonHtml = $@"<a href=""{loginUrl}"" class=""button"">🚀 Đăng nhập ngay</a>";

            return GetBaseTemplate("Đặt lại mật khẩu thành công", content, buttonHtml);
        }
    }
}