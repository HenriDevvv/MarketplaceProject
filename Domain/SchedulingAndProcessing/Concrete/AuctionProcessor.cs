using DAL.Concrete;
using DAL.Contracts;
using DAL.UoW;
using Domain.SchedulingAndProcessing.Contracts;
using Entities.Models;
using Lamar;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.SchedulingAndProcessing.Concrete
{
    internal class AuctionProcessor : IAuctionProcessor
    {
        private IServiceProvider _serviceProvider;
        public AuctionProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public void EndAuction(Guid auctionId)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();
                var _auctionListingRepository = unitOfWork.GetRepository<IAuctionListingRepository>();
                var _userRepository = unitOfWork.GetRepository<IUserRepository>();
                var _saleRepository = unitOfWork.GetRepository<ISaleRepository>();
                var auction = _auctionListingRepository.GetById(auctionId);
                if (auction != null)
                {
                    if (auction.CurrentHighestBid == null)
                    {
                        _auctionListingRepository.Remove(auction);
                        unitOfWork.Save();
                        NotifySellerOfAuctionEndingWithNoBids(auction.Name, auction.Seller.Email, auction.Seller.Username);
                        return;
                    }

                    auction.Seller.Balance += auction.CurrentHighestBid.Amount;
                    _userRepository.Update(auction.Seller);
                    _auctionListingRepository.Remove(auction);
                    _saleRepository.Add(new Sale
                    {
                        Buyer = auction.CurrentHighestBid.User,
                        Time = DateTime.Now,
                        Amount = auction.CurrentHighestBid.Amount,
                        Listing = auction
                    });
                    unitOfWork.Save();
                    NotifySellerOfAuctionEnding(auction.Name, auction.Seller.Email, auction.Seller.Username, auction.CurrentHighestBid.User.Username, auction.CurrentHighestBid.Amount);
                    NotifyBuyerOfAuctionEnding(auction.Name, auction.CurrentHighestBid.User.Username, auction.CurrentHighestBid.User.Email, auction.CurrentHighestBid.Amount);
                }
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
    }
}
