using Dapper;
using DapperDashboard.ViewModels;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DapperDashboard.Extensions;

namespace DapperDashboard.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: /Invoice/ or /Invoice/Index?page=2
        [HttpGet]
        public async Task<ActionResult> Index(int page = 1)
        {
            const int PageSize = 10;
            if (page < 1) page = 1;
            var offset = (page - 1) * PageSize;
            var loggedInCustomerId = User.Identity.GetCustomerId();

            var sql = @"
                SELECT COUNT(InvoiceID) FROM Invoices WHERE CustomerID = @CustId;
                SELECT InvoiceReference, IssueDate, DueDate, Amount, [Status]
                FROM Invoices
                WHERE CustomerID = @CustId
                ORDER BY IssueDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            var viewModel = new InvoiceListViewModel
            {
                Pagination = new PaginationInfoViewModel { CurrentPage = page, PageSize = PageSize }
            };

            using (var connection = new SqlConnection(_connStr))
            {
                await connection.OpenAsync();
                using (var multi = await connection.QueryMultipleAsync(sql, new { CustId = loggedInCustomerId, Offset = offset, PageSize = PageSize }))
                {
                    viewModel.Pagination.TotalItems = await multi.ReadSingleAsync<int>();
                    viewModel.Invoices = (await multi.ReadAsync<InvoiceViewModel>()).ToList();
                }
            }
            return View(viewModel);
        }

        // GET: Invoice/Details/{ref}
        public async Task<ActionResult> Details(string @ref)
        {
            if (string.IsNullOrEmpty(@ref))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var loggedInCustomerId = User.Identity.GetCustomerId();
            var sql = @"
                SELECT I.InvoiceID, I.InvoiceReference, I.IssueDate, I.DueDate, I.[Status], C.CustomerName
                FROM Invoices I JOIN Customers C ON I.CustomerID = C.CustomerID
                WHERE I.InvoiceReference = @InvoiceRef AND I.CustomerID = @CustId;

                SELECT P.ProductName, IL.Quantity, IL.UnitPrice
                FROM InvoiceLines IL JOIN Products P ON IL.ProductID = P.ProductID JOIN Invoices I ON IL.InvoiceID = I.InvoiceID
                WHERE I.InvoiceReference = @InvoiceRef AND I.CustomerID = @CustId
                ORDER BY P.ProductName;
            ";

            using (var connection = new SqlConnection(_connStr))
            {
                await connection.OpenAsync();
                using (var multi = await connection.QueryMultipleAsync(sql, new { InvoiceRef = @ref, CustId = loggedInCustomerId }))
                {
                    var viewModel = await multi.ReadSingleOrDefaultAsync<InvoiceDetailViewModel>();
                    if (viewModel == null)
                    {
                        return HttpNotFound();
                    }
                    viewModel.LineItems = (await multi.ReadAsync<InvoiceLineItemViewModel>()).ToList();
                    return View(viewModel);
                }
            }
        }
    }
}