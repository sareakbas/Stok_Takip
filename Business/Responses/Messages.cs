namespace Business.Responses 
{
    public static class Messages
    {
        // Müşteri (Customer) Mesajları
        public const string CustomerListed = "Müşteriler başarıyla listelendi.";
        public const string CustomerAlreadyExists = "Bu e-posta veya telefon numarasına sahip bir müşteri zaten sistemde kayıtlı.";
        public const string CustomerAdded = "Müşteri başarıyla eklendi.";
        public const string CustomerNotFound = "İşlem yapılmak istenen müşteri bulunamadı.";
        public const string CustomerUpdated = "Müşteri başarıyla güncellendi.";
        public const string CustomerDeleted = "Müşteri başarıyla pasife alındı.";

        // Auth (Kimlik Doğrulama) Mesajları
        public const string UserAlreadyExists = "Bu e-posta adresi zaten sistemde kayıtlı!";
        public const string UserRegistered = "Kayıt işlemi başarıyla tamamlandı! Yönetici onayından sonra giriş yapabilirsiniz.";
        public const string UserNotFound = "Kullanıcı bulunamadı.";
        public const string UserAccountLocked = "Hesabınız kilitlendi. Lütfen {0} sonrasında tekrar deneyin.";
        public const string UserNotActive = "Hesabınız henüz onaylanmamış. Lütfen yöneticinizle iletişime geçin.";
        public const string UserLockedDueToFailedAttempts = "Üst üste 5 kez hatalı giriş yaptığınız için hesabınız 15 dakika kilitlendi.";
        public const string PasswordError = "Şifre yanlış.";
        public const string LoginSuccessful = "Giriş başarılı.";
        public const string UnauthorizedAccess = "Yetkisiz işlem. Geçerli bir kullanıcı bulunamadı.";

        // Ürün (Product) Mesajları
        public const string ProductListed = "Ürünler başarıyla listelendi.";
        public const string ProductBarcodeAlreadyExists = "Bu barkoda sahip bir ürün zaten mevcut. Aynı barkodla ikinci bir ürün eklenemez.";
        public const string ProductAdded = "Ürün başarıyla eklendi.";
        public const string ProductNotFound = "İşlem yapılmak istenen ürün bulunamadı.";
        public const string ProductBarcodeUsedByAnother = "Bu barkod başka bir ürün tarafından kullanılıyor.";
        public const string ProductUpdated = "Ürün başarıyla güncellendi.";
        public const string ProductDeactivated = "Ürün başarıyla pasife alındı ve listelerden kaldırıldı.";
        public const string ProductAlreadyActive = "Bu ürün zaten aktif durumda.";
        public const string ProductReactivated = "Ürün başarıyla tekrar aktifleştirildi ve listelere eklendi.";

        // Tedarikçi (Supplier) Mesajları
        public const string SupplierListed = "Tedarikçiler başarıyla listelendi.";
        public const string SupplierAlreadyExists = "Bu e-posta veya telefon numarasına sahip bir tedarikçi zaten sistemde kayıtlı.";
        public const string SupplierAdded = "Tedarikçi başarıyla eklendi.";
        public const string SupplierNotFound = "İşlem yapılmak istenen tedarikçi bulunamadı.";
        public const string SupplierUpdated = "Tedarikçi başarıyla güncellendi.";
        public const string SupplierDeleted = "Tedarikçi başarıyla pasife alındı.";

        // Stok (Stock) Mesajları
        public const string StockEntryProductNotFound = "Aktif bir ürün bulunamadı. Stok girişi yapılamaz.";
        public const string StockEntrySuccessful = "{0} için {1} adet stok girişi başarıyla yapıldı ve Lot oluşturuldu.";
        public const string StockEntryFailed = "Stok girişi sırasında beklenmeyen sistemsel bir hata oluştu: {0}";

    }
}