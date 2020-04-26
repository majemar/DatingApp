using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            //pobieramy z bazy danych usera o nazwe username przekazanej do metody
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
            
            //sprawdzamy czy user istnieje w bazie danych
            if (user == null)
                return null;

            //weryfikujemy za pomocą metody VerifyPasswordHash czy hasło przekazane do motody jest identyczne z zapisanym w bazie danych
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            
            return user;           
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //wywołanie metody deszyfrującej hasło - odwracamy proces wywołany w metodzie CreatePasswordHash
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var computedHash = hmac.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));

                //porówunjemy ze sobą hasło zapisane w bazie danych z hasłem przysłanym - porównujemy znak po znaku
                for (int i = 0; i<computedHash.Length; i++){
                    if (computedHash[i] != passwordHash[i]) 
                        return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
           //n/w 6 wersów odpowiada za wywołanie metody szyfrującej hasło oraz zapisanie hasła do bazy danych 
           byte[] passwordHash, passwordSalt;

           CreatePasswordHash(password, out passwordHash, out passwordSalt);

           user.PasswordHash = passwordHash;
           user.PasswordSalt = passwordSalt;

           await _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();

           return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //wywołanie metody szyfrującej hasło 
            using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
                return true;

            return false;
        }
    }
}