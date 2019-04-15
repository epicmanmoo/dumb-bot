using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace botTesting
{
    public static class Data
    {
        public static int GetStones(ulong UserId)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.Stones.Where(x => x.UserId == UserId).Count() < 1)
                {
                    return 0;
                }
                return DbContext.Stones.Where(x => x.UserId == UserId).Select(x => x.Amount).FirstOrDefault();
            }
        }
        public static async Task SaveStones(ulong UserId, int Amount)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.Stones.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Add(new Stone
                    {
                        UserId = UserId,
                        Amount = Amount
                    });
                }
                else
                {
                    Stone Current = DbContext.Stones.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Amount += Amount;
                    DbContext.Stones.Update(Current);
                }
                await DbContext.SaveChangesAsync();
            }          
        }
        public static async Task BuyDogs(ulong UserId)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                              
                Stone Dogs = DbContext.Stones.Where(x => x.UserId == UserId).FirstOrDefault();
                Dogs.Item1++;
                DbContext.Stones.Update(Dogs);
                await DbContext.SaveChangesAsync();
            }
        }
        //9 more methods for store
    }
}

