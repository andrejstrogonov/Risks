using Microsoft.EntityFrameworkCore;
using SQLiteLibrary.DataAccess;
using System.Threading;
using System.Windows.Forms;

namespace SQLiteLibrary.Model
{
    public class SampleContext : DbContext
    {
        public static string Path = @"Data Source=RisksDB.db";
        public DbSet<ProductNullable> ProductsNullable { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<TransferOfUnits> TransfersOfUnits { get; set; }
        public DbSet<ResourceNullable> ResourcesNullable { get; set; }
        public DbSet<ResourceOfProductNullable> ResourcesOfProductsNullable { get; set; }
        public DbSet<RiskOfProductNullable> RiskOfProductsNullable { get; set; }
        public DbSet<FixedCost> FixedCosts { get; set; }
        public DbSet<GroupOfInterchangableNullable> GroupOfInterchangablesNullable { get; set; }
        public DbSet<MemberOfGroupNullable> MemberOfGroupsNullable { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<UnitSequence> UnitSequences { get; set; }

        private static Mutex Mutex = new Mutex();
        public static bool IsBusy = false;
        private static int UsingNow = 0;

        private static Thread CurrentThread = null;
        public SampleContext(bool _created = true)
        {
            if(CurrentThread is null)
            {               
                Mutex.WaitOne();
                CurrentThread = Thread.CurrentThread; 
            }
            else
            {
                if (CurrentThread != Thread.CurrentThread)
                {
                    Mutex.WaitOne();
                    CurrentThread = Thread.CurrentThread;
                   
                }
            }
            UsingNow++;
            if (!IsBusy)
            {
                var a = 3;
            }
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl());


            SQLitePCL.Batteries_V2.Init();
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
            
            //MessageBox.Show("sampleCOntext");
            if (!_created)
            {
                _created = true;                
                Database.EnsureDeleted();
                Database.EnsureCreated();
                UnitSequence unitSequence_1 = new UnitSequence()
                {
                    Name = "Время",
                    IsUsing = true,
                    TypeOfUnit = TypeOfUnit.Time
                };

                UnitSequenceDAL.SetUnitSequence(unitSequence_1);

                Unit unit_1 = new Unit()
                {
                    Symbol = "день",
                    Name = "День",
                    IsBasis=true,
                    UnitSequence_ID = unitSequence_1.ID
                };
                Unit unit_2 = new Unit()
                {
                    Symbol = "неделя",
                    Name = "Неделя",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_1.ID
                };
                Unit unit_3 = new Unit()
                {
                    Symbol = "месяц",
                    Name = "Месяц",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_1.ID
                };

                Unit unit_4 = new Unit()
                {
                    Symbol = "сезон",
                    Name = "Сезон",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_1.ID
                };

                Unit unit_5 = new Unit()
                {
                    Symbol = "год",
                    Name = "Год",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_1.ID
                };

                UnitDAL.AddUnit(unit_1);
                UnitDAL.AddUnit(unit_2);
                UnitDAL.AddUnit(unit_3);
                UnitDAL.AddUnit(unit_4);
                UnitDAL.AddUnit(unit_5);

                TransferOfUnits transferOfUnits_1 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_2.ID,
                    ID_UnitTo = unit_1.ID,
                    Coefficient = 7
                };
                TransferOfUnits transferOfUnits_2 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_3.ID,
                    ID_UnitTo = unit_1.ID,
                    Coefficient = 30
                };
                TransferOfUnits transferOfUnits_3 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_4.ID,
                    ID_UnitTo = unit_1.ID,
                    Coefficient = 90
                };
                TransferOfUnits transferOfUnits_4 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_5.ID,
                    ID_UnitTo = unit_1.ID,
                    Coefficient = 365
                };

                TransferOfUnits transferOfUnits_5 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_3.ID,
                    ID_UnitTo = unit_2.ID,
                    Coefficient = 4
                };

                TransferOfUnits transferOfUnits_6 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_4.ID,
                    ID_UnitTo = unit_2.ID,
                    Coefficient = 12
                };

                TransferOfUnits transferOfUnits_7 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_5.ID,
                    ID_UnitTo = unit_2.ID,
                    Coefficient = 48
                };

                TransferOfUnits transferOfUnits_8 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_4.ID,
                    ID_UnitTo = unit_3.ID,
                    Coefficient = 3
                };


                TransferOfUnits transferOfUnits_9 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_5.ID,
                    ID_UnitTo = unit_3.ID,
                    Coefficient = 12
                };

                TransferOfUnits transferOfUnits_10 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit_5.ID,
                    ID_UnitTo = unit_4.ID,
                    Coefficient = 4
                };

                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_1);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_2);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_3);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_4);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_5);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_6);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_7);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_8);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_9);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits_10);

                UnitSequence unitSequence_2 = new UnitSequence()
                {
                    Name = "Масса",
                    IsUsing = true,
                    TypeOfUnit = TypeOfUnit.Mass
                };

                UnitSequenceDAL.SetUnitSequence(unitSequence_2);

                Unit unit1 = new Unit()
                {
                    Symbol = "кг",
                    Name = "Килограмм",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_2.ID
                };
                Unit unit2 = new Unit()
                {
                    Symbol = "г",
                    Name = "Грамм",
                    IsBasis = true,
                    UnitSequence_ID = unitSequence_2.ID
                };


                UnitSequence unitSequence_3 = new UnitSequence()
                {
                    Name = "Объем",
                    IsUsing = true,
                    TypeOfUnit = TypeOfUnit.Volume
                };
                UnitSequenceDAL.SetUnitSequence(unitSequence_3);

                Unit unit3 = new Unit()
                {
                    Symbol = "л",
                    Name = "Литр",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_3.ID
                };
                Unit unit4 = new Unit()
                {
                    Symbol = "мл",
                    Name = "Миллилитр",
                    IsBasis = true,
                    UnitSequence_ID = unitSequence_3.ID
                };


                UnitSequence unitSequence_4 = new UnitSequence()
                {
                    Name = "Целые единицы",
                    IsUsing = true,
                    TypeOfUnit = TypeOfUnit.Unit
                };
                UnitSequenceDAL.SetUnitSequence(unitSequence_4);

                Unit unit5 = new Unit()
                {
                    Symbol = "шт",
                    Name = "Штуки",
                    IsBasis = true,
                    UnitSequence_ID = unitSequence_4.ID
                };

                UnitSequence unitSequence_5 = new UnitSequence()
                {
                    Name = "Деньги",
                    IsUsing = true,
                    TypeOfUnit = TypeOfUnit.Money
                };
                UnitSequenceDAL.SetUnitSequence(unitSequence_5);
                Unit unit6 = new Unit()
                {
                    Symbol = "руб.",
                    Name = "Рубль",
                    IsBasis = true,
                    UnitSequence_ID = unitSequence_5.ID
                };
                Unit unit7 = new Unit()
                {
                    Symbol = "тыс.руб.",
                    Name = "Тысяч рублей",
                    IsBasis = false,
                    UnitSequence_ID = unitSequence_5.ID
                };
                UnitDAL.AddUnit(unit1);
                UnitDAL.AddUnit(unit2);
                UnitDAL.AddUnit(unit3);
                UnitDAL.AddUnit(unit4);
                UnitDAL.AddUnit(unit5);
                UnitDAL.AddUnit(unit6);
                UnitDAL.AddUnit(unit7);
                TransferOfUnits transferOfUnits1 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit1.ID,
                    ID_UnitTo = unit2.ID,
                    Coefficient = 1000
                };
                TransferOfUnits transferOfUnits2 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit3.ID,
                    ID_UnitTo = unit4.ID,
                    Coefficient = 1000
                };
                TransferOfUnits transferOfUnits3 = new TransferOfUnits()
                {
                    ID_UnitFrom = unit7.ID,
                    ID_UnitTo = unit6.ID,
                    Coefficient = 1000
                };
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits1);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits2);
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits3);                
            }
        }


        public override void Dispose()
        {
            UsingNow--;
            if (UsingNow==0 && CurrentThread==Thread.CurrentThread)
            {
                CurrentThread = null;
                Mutex.ReleaseMutex();
            }
            else
            {
                var a = 3;
            }
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            //MessageBox.Show("onconfiguring");
            optionbuilder.UseSqlite(Path);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitSequence>()
                .HasKey(c => c.ID);

            modelBuilder.Entity<Settings>()
                .HasKey(c => c.ID);

            modelBuilder.Entity<Settings>()
                .HasOne(c => c.PeriodUnit)
                .WithMany(u => u.PeriodUnits)
                .HasForeignKey(c => c.PeriodUnitID);

            modelBuilder.Entity<Settings>()
                .HasOne(c => c.BudgetUnit)
                .WithMany(u => u.BudgetUnits)
                .HasForeignKey(c => c.BudgetUnitID);

            modelBuilder.Entity<Unit>()
                .HasOne(c => c.UnitSequence)
                .WithMany(c => c.Units)
                .HasForeignKey(c => c.UnitSequence_ID);

            modelBuilder.Entity<ProductNullable>()
                .HasOne(c => c.UnitOfMinimum)
                .WithMany(u => u.ProductsMinimum)
                .HasForeignKey(c => c.UnitOfMimimumID);
            modelBuilder.Entity<ProductNullable>()
                .HasOne(c => c.UnitOfMaximum)
                .WithMany(u => u.ProductsMaximum)
                .HasForeignKey(c=>c.UnitOfMaximumID);
            modelBuilder.Entity<ProductNullable>()
                .HasOne(c => c.UnitOfStock)
                .WithMany(u => u.ProductsStock)
                .HasForeignKey(c=>c.UnitOfStockID);
            modelBuilder.Entity<ProductNullable>()
                .HasOne(c => c.UnitOfOther)
                .WithMany(u => u.ProductsOther)
                .HasForeignKey(c => c.UnitOfOtherID);

            modelBuilder.Entity<ResourceNullable>()
                .HasOne(c => c.UnitOfPrice)
                .WithMany(u => u.ResourcesPrice)
                .HasForeignKey(c => c.UnitOfPriceID);
            modelBuilder.Entity<ResourceNullable>()
                .HasOne(c => c.UnitOfMaximum)
                .WithMany(u => u.ResourcesMaximum)
                .HasForeignKey(c => c.UnitOfMaximumID);
            modelBuilder.Entity<ResourceNullable>()
                .HasOne(c => c.UnitOfStock)
                .WithMany(u => u.ResourcesStock)
                .HasForeignKey(c => c.UnitOfStockID);

            modelBuilder.Entity<TransferOfUnits>()
                .HasKey(c => new { c.ID_UnitFrom, c.ID_UnitTo });
                

            modelBuilder.Entity<TransferOfUnits>()
                .HasOne(c => c.UnitFrom)
                .WithMany(u => u.TransferOfUnitsFrom)
                .HasForeignKey(c => c.ID_UnitFrom);
            modelBuilder.Entity<TransferOfUnits>()
                .HasOne(c => c.UnitTo)
                .WithMany(u => u.TransferOfUnitsTo)
                .HasForeignKey(c => c.ID_UnitTo);

            modelBuilder.Entity<ResourceOfProductNullable>()
                .HasOne(c => c.ProductNullable)
                .WithMany(c => c.ResourcesOfProduct)
                .HasForeignKey(c => c.ID_Product);

            modelBuilder.Entity<ResourceOfProductNullable>()
                .HasOne(c => c.ResourceNullable)
                .WithMany(c => c.ResourceOfProducts)
                .HasForeignKey(c => c.ID_Resource);


            modelBuilder.Entity<RiskOfProductNullable>()
                .HasOne(c => c.ProductNullable)
                .WithMany(c => c.RisksOfProduct)
                .HasForeignKey(c => c.ID_Product);
            modelBuilder.Entity<RiskOfProductNullable>()
                .HasOne(c => c.UnitOfAmount)
                .WithMany(c => c.RiskOfProductsAmount)
                .HasForeignKey(c => c.UnitOfAmountID);
            modelBuilder.Entity<RiskOfProductNullable>()
                .HasOne(c => c.UnitOfPrice)
                .WithMany(c => c.RiskOfProductsPrice)
                .HasForeignKey(c => c.UnitOfPriceID);

            modelBuilder.Entity<FixedCost>()
               .HasOne(c => c.UnitOfPrice)
               .WithMany(c => c.FixedCosts)
               .HasForeignKey(c => c.UnitOfPriceID);

            modelBuilder.Entity<GroupOfInterchangableNullable>()
                .HasKey(c => c.ID);

            modelBuilder.Entity<GroupOfInterchangableNullable>()
                .HasOne(c => c.UnitOfMaximum)
                .WithMany(c => c.GroupOfInterchangableMaximum)
                .HasForeignKey(c => c.UnitOfMaximumID);

            modelBuilder.Entity<GroupOfInterchangableNullable>()
                .HasOne(c => c.UnitOfMinimum)
                .WithMany(c => c.GroupOfInterchangableMinimum)
                .HasForeignKey(c => c.UnitOfMinimumID);

            modelBuilder.Entity<MemberOfGroupNullable>()
                .HasKey(c => new { c.ID_GroupOfInterchangable, c.ID_Product });

            modelBuilder.Entity<MemberOfGroupNullable>()
                .HasOne(c => c.GroupOfInterchangableNullable)
                .WithMany(c => c.MembersOfGroup)
                .HasForeignKey(c => c.ID_GroupOfInterchangable);

            modelBuilder.Entity<MemberOfGroupNullable>()
                .HasOne(c => c.ProductNullable)
                .WithMany(c => c.MembersOfGroup)
                .HasForeignKey(c => c.ID_Product);
            
        }
        
    }

}
