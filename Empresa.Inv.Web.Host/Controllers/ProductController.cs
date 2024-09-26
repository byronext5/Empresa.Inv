using AutoMapper;
using Empresa.Inv.Application.Shared.Dtos;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.EntityFramework.Core;
using Empresa.Inv.Web.Host.Authorization;
using Empresa.Inv.Web.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace Empresa.Inv.Web.Host.Controllers
{

    //[ApiController]
    //// [Authorize]
    //[Route("api/[controller]")]
    //public class ProductController : ControllerBase
    //{
    //    protected readonly ApplicationDbContext _context;
    //    public ProductController(ApplicationDbContext context)
    //    {
    //        _context = context;

    //    }
    //    [HttpGet("GetProducts")]
    //    public IQueryable<Product> GetAll()
    //    {
    //        var lista = _context.Products.AsQueryable();
    //        return lista;
    //    }

    //}


    [ApiController]
    //  [Authorize]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly CacheService _cacheService;

        public ProductController(
            IMapper mapper,
            ILogger<ProductController> logger,
            CacheService cacheService,
            IRepository<Product> productsRepository,
            IProductRepository productRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Category> categoriRepository)
        {

            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
            // reporsitorios genericos
            _productsRepository = productsRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoriRepository;
            _cacheService = cacheService;

        }
        /*
        [HttpGet("GetProducts")]
        public IQueryable<Product> GetAll()
        {
            var lista = _productsRepository.GetAll();

            var listareal = lista.ToList();

            return lista;
        } */

        [CustomAuthorize(AppPermissions.Pages_General_Data)]
        [HttpGet("GetProducts")]

        public List<Product> GetAll()
        {
            const string cacheKey = "GetAllData";
            var data = _cacheService.Get<List<Product>>(cacheKey);
            List<Product> products = new List<Product>();
            if (data == null)
            {
                var lista = _productsRepository.GetAll();
                products = lista.ToList();
                // Establecer datos en caché por 10 minutos
                _cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(10));
            }
            else
                products = data;
            return products;
        }


        [HttpGet("GetProductsEf")]
        public async Task<IActionResult> GetProductsEf(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncEf(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }

        [HttpGet("GetProductsSp")]
        public async Task<IActionResult> GetProductsSp(
           [FromQuery] string searchTerm,
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }
        // NUEVAS FUNCIONES
        ////[HttpGet("{id}")]
        ////public async Task<IActionResult> GetProduct2(int id) 
        ////{ 
        ////    var product = await _productRepository.GetByIdAsync(id);
        ////    if(product == null) return NotFound();  

        ////    var productDto = _mapper.Map<Product>(product);
        ////    return Ok(productDto);
        ////}


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetProductNames(int id)
        //{
        //    var product = await _productsRepository
        //        .GetAll()
        //        .Where(i =>i.Id == id).FirstOrDefaultAsync();

        //    ProductDTO resultado = new ProductDTO();
        //    Category category = new Category();
        //    Supplier supplier = new Supplier(); 

        //    try
        //    {
        //        if (product != null) {

        //            category = await _categoryRepository
        //            .GetAll()
        //            .Where(i => i.Id == product.CategoryId).FirstOrDefaultAsync();

        //            supplier = await _supplierRepository
        //                .GetAll()
        //                .Where(i => i.Id == product.SupplierId).FirstOrDefaultAsync();
        //        }
                               
        //        resultado.Id = product.Id;
        //        resultado.Name = product.Name;
        //        resultado.CategoryName = category.Name;
        //        resultado.SupplierName = supplier.Name;
        //    }
        //    catch (Exception exx)
        //    {

        //        throw;
        //    }

        //    return  Ok(resultado);  

        //    /*
        //    if (product == null) return NotFound();

        //    var productDto = _mapper.Map<Product>(product);
        //    */

        //    //return Ok(productDto);
        //}

        [HttpGet("{id}")]
        public async Task<ProductDTO> GetProductDetailsByIdAsync( int id)
        {
            ProductDTO resultado = new ProductDTO();
            resultado = await _productRepository.GetProductsDetailsByIdMapperAsync(id);
            
            return resultado;

            /*
            if (product == null) return NotFound();

            var productDto = _mapper.Map<Product>(product);
            */

            //return Ok(productDto);
        }


        [HttpGet("error")]
        public IActionResult Error()
        {
            _logger.LogInformation("Este es un mensaje de prueba desde TestController, antes del error.");
            _logger.LogDebug("Mensaje de depuración");
            _logger.LogWarning("Mensaje de advertencia");
            _logger.LogError("Mensaje de error");
            try
            {
                // Genera una excepción de prueba
                throw new Exception("Excepción de prueba en el controlador");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Hello World");
                // Usar el logger para registrar errores
                _logger.LogError(ex, "Ocurrió un error en la API.");
                return StatusCode(500, "Error interno en el servidor");
            }
        }


        [HttpPost("recordTransaction")]
        public async Task<IActionResult> RecordProductTransaction([FromBody] ProductTransactionRequest request)
        {
            if (request == null || request.Amonunt <=0) 
            {
                return BadRequest("Invalid request");
            }
            try
            {
                await _productRepository.UpdateInventAsync(request.ProductoId, request.Amonunt, request.UserId, request.TypeId);
                return Ok("transaccion correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                return StatusCode(500, "Internal server error");
                
            }
            
        }

        [HttpGet("kardex-summary")]
        public async Task<IActionResult> getSummaryKardex([FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {
            if (startDate > endDate )
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha final");
            }
            
            var resultado = await _productRepository.getKardexSummaryByUserAsync(startDate, endDate);

            return Ok(resultado);            

        }


    }


}

