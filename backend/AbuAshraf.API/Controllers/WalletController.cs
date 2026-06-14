using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Queries;
using AbuAshraf.Application.Services;

namespace AbuAshraf.API.Controllers
{
    /// <summary>
    /// Wallet management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWalletService _walletService;

        public WalletController(IMediator mediator, IWalletService walletService)
        {
            _mediator = mediator;
            _walletService = walletService;
        }

        /// <summary>
        /// Get wallet balance
        /// </summary>
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var wallet = await _walletService.GetWalletAsync(userId);

                if (wallet == null)
                    return NotFound(new { Success = false, Message = "Wallet not found" });

                return Ok(new { Success = true, Data = wallet });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get available balance
        /// </summary>
        [HttpGet("available-balance")]
        public async Task<IActionResult> GetAvailableBalance()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var availableBalance = await _walletService.GetAvailableBalanceAsync(userId);

                return Ok(new { Success = true, Data = new { AvailableBalance = availableBalance } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get wallet dashboard summary
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var query = new GetWalletDashboardQuery(userId);
                var dashboard = await _mediator.Send(query);

                if (dashboard == null)
                    return NotFound(new { Success = false, Message = "Dashboard data not found" });

                return Ok(new { Success = true, Data = dashboard });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get transaction history
        /// </summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var query = new GetTransactionHistoryQuery(userId, pageNumber, pageSize);
                var history = await _mediator.Send(query);

                return Ok(new { Success = true, Data = history });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get transaction by reference number
        /// </summary>
        [HttpGet("transactions/{referenceNumber}")]
        public async Task<IActionResult> GetTransaction(string referenceNumber)
        {
            try
            {
                var transaction = await _walletService.GetTransactionByReferenceAsync(referenceNumber);

                if (transaction == null)
                    return NotFound(new { Success = false, Message = "Transaction not found" });

                return Ok(new { Success = true, Data = transaction });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Fund wallet
        /// </summary>
        [HttpPost("fund")]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletDto dto)
        {
            try
            {
                if (dto.Amount <= 0)
                    return BadRequest(new { Success = false, Message = "Amount must be greater than zero" });

                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var command = new FundWalletCommand(userId, dto.Amount, dto.PaymentGateway);
                var result = await _mediator.Send(command);

                return result
                    ? Ok(new { Success = true, Message = "Wallet funded successfully" })
                    : BadRequest(new { Success = false, Message = "Failed to fund wallet" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Check balance sufficiency
        /// </summary>
        [HttpPost("verify-balance")]
        public async Task<IActionResult> VerifyBalance([FromBody] VerifyBalanceDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var hasSufficientBalance = await _walletService.HasSufficientBalanceAsync(userId, dto.Amount);
                var withinDailyLimit = await _walletService.IsWithinDailyLimitAsync(userId, dto.Amount);
                var withinMonthlyLimit = await _walletService.IsWithinMonthlyLimitAsync(userId, dto.Amount);

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        HasSufficientBalance = hasSufficientBalance,
                        WithinDailyLimit = withinDailyLimit,
                        WithinMonthlyLimit = withinMonthlyLimit,
                        CanProceed = hasSufficientBalance && withinDailyLimit && withinMonthlyLimit
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
