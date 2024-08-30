using DAL.Contracts;
using DAL.UoW;
using Domain.Contracts;
using DTO.AuctionListingDTO;
using DTO.BidDTO;
using Entities.Models;
using Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Lamar;
using DAL.DI;
using Microsoft.Extensions.DependencyInjection;
using DAL.Concrete;
using Microsoft.EntityFrameworkCore;
using Domain.SchedulingAndProcessing.Contracts;

namespace Domain.Concrete
{
    internal class AuctionListingDomain : DomainBase, IAuctionListingDomain
    {
        private IAuctionScheduler _auctionScheduler;
        private IServiceProvider _serviceProvider;
        public AuctionListingDomain(IUnitOfWork unitOfWork, IAuctionScheduler auctionScheduler, IServiceProvider serviceProvider) : base(unitOfWork)
        {
            _auctionScheduler = auctionScheduler;
            _serviceProvider = serviceProvider; 
        }
        private IAuctionListingRepository _auctionListingRepository => _unitOfWork.GetRepository<IAuctionListingRepository>();
        private IUserRepository _userRepository => _unitOfWork.GetRepository<IUserRepository>();
        private ISaleRepository _saleRepository => _unitOfWork.GetRepository<ISaleRepository>();
        private System.Threading.Timer? _checker;

        public bool CreateBid(BidCreateDTO dto)
        {
            var user = _userRepository.GetById(dto.UserId);
            var auctionListing = _auctionListingRepository.GetById(dto.AuctionListingId);
            if (user == null)
                throw new Exception("User does not exist");
            else if (auctionListing == null)
                throw new Exception("Listing does not exist");
            else if (user.Balance < dto.Amount)
                throw new Exception("User balance is not sufficient");
            else if (DateTime.Now.CompareTo(auctionListing.StartTime) < 0)
                throw new Exception("Bidding has not started yet");
            else if (DateTime.Now.CompareTo(auctionListing.EndTime) > 0)
                throw new Exception("Bidding has ended");

            Console.WriteLine($"{DateTime.Now}  {auctionListing.StartTime} {auctionListing.EndTime}");

            if (auctionListing.CurrentHighestBid == null && dto.Amount > auctionListing.StartingPrice)
            {
                auctionListing.CurrentHighestBid = new Bid
                {
                    UserId = user.Id,
                    User = user,
                    Amount = dto.Amount,
                    TimePlaced = DateTime.Now,
                };
                user.Balance -= dto.Amount;

                _userRepository.Update(user);
                _auctionListingRepository.Update(auctionListing);
                _unitOfWork.Save();

                NotifySellerForNewBid(auctionListing.Name, auctionListing.Seller.Email, auctionListing.Seller.Username, user.Username, dto.Amount);
                return true;
            }
            else if (auctionListing.CurrentHighestBid != null && auctionListing.CurrentHighestBid.Amount < dto.Amount)
            {
                var lastHighBidder = auctionListing.CurrentHighestBid.User;
                lastHighBidder.Balance += auctionListing.CurrentHighestBid.Amount;
                auctionListing.CurrentHighestBid = new Bid
                {
                    UserId = user.Id,
                    User = user,
                    Amount = dto.Amount,
                    TimePlaced = DateTime.Now,
                };
                user.Balance -= dto.Amount;
                
                _userRepository.Update(lastHighBidder);
                _userRepository.Update(user);
                _auctionListingRepository.Update(auctionListing);
                _unitOfWork.Save();
                return true;
            }
            else
                return false;
        }

        public Guid CreateListing(AuctionListingCreateDTO dto)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var seller = _userRepository.GetById(dto.SellerId);
                var newListing = new AuctionListing
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    SellerId = dto.SellerId,
                    Seller = seller,
                    Status = (int)DeleteSatus.Active,
                    StartingPrice = dto.StartingPrice,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };
                var listing = _auctionListingRepository.Add(newListing);
                _unitOfWork.Save();

                Task.Run(() => _auctionScheduler.ScheduleAuctionEnd(listing.Id, listing.EndTime, scope.ServiceProvider));

                return listing.Id; 
            }
        }

        public bool DeleteListing(Guid id)
        {
            var entity = _auctionListingRepository.GetById(id);
            if(entity == null) 
            {
                return false;
            }
            _auctionListingRepository.Remove(entity);
            _unitOfWork.Save();
            return true;
        }

        public List<AuctionListingReadDTO> GetAllListings()
        {
            var entities = _auctionListingRepository.GetAll();
            if(!entities.Any())
            {
                return null;
            }
            var dtos = new List<AuctionListingReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new AuctionListingReadDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    SellerId = entity.SellerId,
                    StartingPrice = entity.StartingPrice,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    CurrentHighestBid = entity.CurrentHighestBid == null ? entity.StartingPrice : entity.CurrentHighestBid.Amount
                });
            }
            return dtos;
        }

        public AuctionListingReadDTO GetListing(Guid id)
        {
            var entity = _auctionListingRepository.GetById(id);
            if(entity == null)
            {
                return null;
            }
            var dto = new AuctionListingReadDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                SellerId = entity.SellerId,
                StartingPrice = entity.StartingPrice,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                CurrentHighestBid = entity.CurrentHighestBid == null ? entity.StartingPrice : entity.CurrentHighestBid.Amount
            };
            return dto;
        }

        public List<AuctionListingReadDTO> GetListingsBySellerId(Guid sellerId)
        {
            var entities = _auctionListingRepository.Find(x => x.SellerId.Equals(sellerId));
            if (!entities.Any())
            {
                return null;
            }
            var dtos = new List<AuctionListingReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new AuctionListingReadDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    SellerId = entity.SellerId,
                    StartingPrice = entity.StartingPrice,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    CurrentHighestBid = entity.CurrentHighestBid == null ? entity.StartingPrice : entity.CurrentHighestBid.Amount
                });
            }
            return dtos;
        }

        public bool UpdateListing(AuctionListingUpdateDTO dto)
        {
            var entity = _auctionListingRepository.GetById(dto.Id);
            if(entity == null)
            {
                return false;
            }
            else if(DateTime.Now.CompareTo(dto.StartTime) > 0)
            {
                throw new Exception("Listing cannot be modified because the bidding has started.");
            }
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.StartingPrice = dto.StartingPrice;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            _auctionListingRepository.Update(entity);
            _unitOfWork.Save();
            return true;
        }

        public void EndAuction(Guid auctionId)
        {
            var registry = new ServiceRegistry();
            registry.AddSingleton<DbContext, MarketplaceContext>();
            registry.AddSingleton<IAuctionListingRepository, AuctionListingRepository>();
            registry.AddSingleton<ISaleRepository, SaleRepository>();
            registry.AddSingleton<IUserRepository, UserRepository>();
            var container = new Container(registry);

            var temporaryAuctionRepo = container.GetInstance<IAuctionListingRepository>();
            var temporaryUserRepo = container.GetInstance<IUserRepository>();
            var temporarySaleRepo = container.GetInstance<ISaleRepository>();

            var auction = temporaryAuctionRepo.GetById(auctionId);
            if (auction != null)
            {
                if (auction.CurrentHighestBid == null)
                {
                    temporaryAuctionRepo.Remove(auction);
                    _unitOfWork.Save();
                    NotifySellerOfAuctionEndingWithNoBids(auction.Name, auction.Seller.Email, auction.Seller.Username);
                    return;
                }
                
                auction.Seller.Balance += auction.CurrentHighestBid.Amount;
                temporaryUserRepo.Update(auction.Seller);
                temporaryAuctionRepo.Remove(auction);
                temporarySaleRepo.Add(new Sale
                {
                    Buyer = auction.CurrentHighestBid.User,
                    Time = DateTime.Now,
                    Amount = auction.CurrentHighestBid.Amount,
                    Listing = auction
                });
                _unitOfWork.Save();
                NotifySellerOfAuctionEnding(auction.Name, auction.Seller.Email, auction.Seller.Username, auction.CurrentHighestBid.User.Username, auction.CurrentHighestBid.Amount);
                NotifyBuyerOfAuctionEnding(auction.Name, auction.CurrentHighestBid.User.Username, auction.CurrentHighestBid.User.Email, auction.CurrentHighestBid.Amount);
            }
        }
        private void NotifyBuyerOfAuctionEnding(string listingName, string buyerUsername, string buyerEmail, decimal amount)
        {
            string messageBody = $"Greetings {buyerUsername},\n" +
                $"The auction for {listingName} has ended and you are the highest bidder!\n" +
                $"Bid Amount: {amount}";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("notificationsrep@gmail.com");
                message.To.Add(new MailAddress(buyerEmail));
                message.Subject = "You Have Received a New Bid!";
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("notificationsrep@gmail.com", "atej ciin tgbh qbre");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }
        private void NotifySellerOfAuctionEnding(string listingName, string sellerEmail, string sellerUsername, string bidderUsername, decimal bidAmount)
        {
            string messageBody = $"Greetings {sellerUsername},\n" +
                $"Your {listingName} auction listing has been sold!\n" +
                $"Highest Bidder: {bidderUsername}\n" +
                $"Bid Amount: {bidAmount}";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("notificationsrep@gmail.com");
                message.To.Add(new MailAddress(sellerEmail));
                message.Subject = "You Have Received a New Bid!";
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("notificationsrep@gmail.com", "atej ciin tgbh qbre");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

        private void NotifySellerOfAuctionEndingWithNoBids(string listingName, string sellerEmail, string sellerUsername)
        {
            string messageBody = $"Greetings {sellerUsername},\n" +
                $"Your {listingName} auction listing has received no bids and is removed!\n";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("notificationsrep@gmail.com");
                message.To.Add(new MailAddress(sellerEmail));
                message.Subject = "Your Action Listing Is Removed!";
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("notificationsrep@gmail.com", "atej ciin tgbh qbre");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

        private void NotifySellerForNewBid(string listingName, string sellerEmail, string sellerUsername, string bidderUsername, decimal bidAmount)
        {
            string messageBody = $"Greetings {sellerUsername},\n" +
                $"Your {listingName} auction listing has received a new bid!\n" +
                $"Bidder: {bidderUsername}\n" +
                $"Bid Amount: {bidAmount}";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("notificationsrep@gmail.com");
                message.To.Add(new MailAddress(sellerEmail));
                message.Subject = "You Have Received a New Bid!";
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("notificationsrep@gmail.com", "atej ciin tgbh qbre");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

        
    }
}
