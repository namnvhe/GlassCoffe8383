/*using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cafe.Fontend.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ForgotPasswordController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7151/api/";

            if (!_apiUrl.EndsWith("/"))
            {
                _apiUrl += "/";
            }

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        *//*        [HttpPost]
                public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            var errors = ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                            return Json(new { success = false, errors = errors });
                        }

                        var json = JsonConvert.SerializeObject(request);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync($"{_apiUrl}Auth/forgot-password", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                            return Json(new
                            {
                                success = true,
                                message = result?.message ?? "Email đặt lại mật khẩu đã được gửi thành công!"
                            });
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            var errorResult = JsonConvert.DeserializeObject<dynamic>(errorContent);

                            return Json(new
                            {
                                success = false,
                                message = errorResult?.message ?? "Có lỗi xảy ra khi gửi email đặt lại mật khẩu."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Có lỗi xảy ra: " + ex.Message
                        });
                    }
                }*//*

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    return Json(new
                    {
                        isSuccess = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiUrl}Auth/forgot-password", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    return Json(new
                    {
                        isSuccess = true,
                        message = result?.message ?? "Email đặt lại mật khẩu đã được gửi thành công!"
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    return Json(new
                    {
                        isSuccess = false,
                        message = errorResult?.message ?? "Có lỗi xảy ra khi gửi email đặt lại mật khẩu."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }
    }
}*/

using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cafe.Fontend.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ForgotPasswordController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7151/api/";

            if (!_apiUrl.EndsWith("/"))
            {
                _apiUrl += "/";
            }

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                // Kiểm tra validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    return Json(new
                    {
                        isSuccess = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                // Gửi request đến API
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiUrl}Auth/forgot-password", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Sử dụng strongly-typed model thay vì dynamic
                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                    return Json(new
                    {
                        isSuccess = true,
                        message = result?.Message ?? "Email đặt lại mật khẩu đã được gửi thành công!",
                        data = result?.Data
                    });
                }
                else
                {
                    // Xử lý lỗi từ API
                    try
                    {
                        var errorResult = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                        return Json(new
                        {
                            isSuccess = false,
                            message = errorResult?.Message ?? "Có lỗi xảy ra khi gửi email đặt lại mật khẩu.",
                            errors = errorResult?.Errors
                        });
                    }
                    catch
                    {
                        // Fallback nếu response không đúng format ApiResponse
                        return Json(new
                        {
                            isSuccess = false,
                            message = $"Có lỗi xảy ra (HTTP {(int)response.StatusCode}): {response.ReasonPhrase}",
                            errors = new List<string> { responseContent }
                        });
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.",
                    errors = new List<string> { httpEx.Message }
                });
            }
            catch (TaskCanceledException timeoutEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Yêu cầu bị timeout. Vui lòng thử lại sau.",
                    errors = new List<string> { timeoutEx.Message }
                });
            }
            catch (JsonException jsonEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Lỗi xử lý dữ liệu từ server.",
                    errors = new List<string> { jsonEx.Message }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Có lỗi không mong muốn xảy ra: " + ex.Message,
                    errors = new List<string> { ex.ToString() }
                });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordRequest
            {
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    return Json(new
                    {
                        isSuccess = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                // Kiểm tra password confirmation
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Mật khẩu xác nhận không khớp",
                        errors = new List<string> { "Mật khẩu và xác nhận mật khẩu phải giống nhau" }
                    });
                }

                // Gửi request đến API
                var json = JsonConvert.SerializeObject(new
                {
                    Token = request.Token,
                    NewPassword = request.NewPassword
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiUrl}Auth/reset-password", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                    return Json(new
                    {
                        isSuccess = true,
                        message = result?.Message ?? "Mật khẩu đã được đặt lại thành công!",
                        data = result?.Data
                    });
                }
                else
                {
                    try
                    {
                        var errorResult = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                        return Json(new
                        {
                            isSuccess = false,
                            message = errorResult?.Message ?? "Có lỗi xảy ra khi đặt lại mật khẩu.",
                            errors = errorResult?.Errors
                        });
                    }
                    catch
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            message = $"Có lỗi xảy ra (HTTP {(int)response.StatusCode}): {response.ReasonPhrase}",
                            errors = new List<string> { responseContent }
                        });
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.",
                    errors = new List<string> { httpEx.Message }
                });
            }
            catch (TaskCanceledException timeoutEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Yêu cầu bị timeout. Vui lòng thử lại sau.",
                    errors = new List<string> { timeoutEx.Message }
                });
            }
            catch (JsonException jsonEx)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Lỗi xử lý dữ liệu từ server.",
                    errors = new List<string> { jsonEx.Message }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Có lỗi không mong muốn xảy ra: " + ex.Message,
                    errors = new List<string> { ex.ToString() }
                });
            }
        }
    }
}
