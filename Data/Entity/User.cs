namespace ASP_201.Data.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public String Login { get; set; }
        public String Email { get; set; }
        public String RealName { get; set; }
        public String PasswordHash { get; set; }
        public String PasswirdSalt { get; set; }
        public String? Avatar { get; set; }
        public DateTime? RegisterDt { get; set; }
        public DateTime? LastEnterDt { get; set; }
        public String? EmailCode { get; set; }

        /// 19-04-2023 Profile

        public Boolean IsEmailPublic { get; set; } = false;
        public Boolean IsRealNamePublic { get; set; } = false;
        public Boolean IsDatetimesPublic { get; set; } = false;

    }
}
