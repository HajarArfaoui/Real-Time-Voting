using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace VoteTempsReel.Models
{

    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string CIN { get; set; }
        public string Adresse { get; set; }
        public string Date { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string Voter { get; set; }

        public string Url_image { get; set; }

        

    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Naissance")]
        public string Naissance { get; set; }


        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Me souvenir ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {

        [Required(ErrorMessage = "Username est requis", AllowEmptyStrings = false)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Nom est requis", AllowEmptyStrings = false)]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Prénom est requis", AllowEmptyStrings = false)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Email est requis", AllowEmptyStrings = false)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email n'est pas valide")]
        public string Email { get; set; }
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Le numéro de téléphone est incorrect")]
        [Required(ErrorMessage = "Téléphone est requis", AllowEmptyStrings = false)]
        [Display(Name = "Téléphone")]
        [StringLength(20, ErrorMessage = "le minimum est 10 chiffres", MinimumLength = 10)]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "CIN est requis", AllowEmptyStrings = false)]
        [Display(Name = "CIN")]
        [StringLength(8, ErrorMessage = "CIN se compose de 8 caractères ", MinimumLength = 8)]
        public string CIN { get; set; }
        [Required(ErrorMessage = "Adresse est requis", AllowEmptyStrings = false)]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Date de naissance est requis", AllowEmptyStrings = false)]
        [Display(Name = "Date de Naissance")]
        public string Naissance { get; set; }
        [Required(ErrorMessage = "Mot de passe est requis", AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "{0} doit être de 6 caractères au minimum .", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmation de mot de passe est requis", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Mot de passe et confirmation de mot de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Photo")]
        public string Photo { get; set; }

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
    public class RecupAccountModel
    {

        [Required]
        public string Email { get; set; }
    }
    public class Contact
    {
        [Required]
        public string Prénom { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Sujet { get; set; }
        [Required]
        public string Message{ get; set; }
    }
}
