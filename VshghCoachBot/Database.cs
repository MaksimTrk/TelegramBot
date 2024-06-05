using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using APIprot;

namespace VshghCoachBot
{
    public class Database
    {
        NpgsqlConnection con = new NpgsqlConnection(Constant.Connect);
        public async Task InsertUserParameters(long userId, double height, double weight)
        {

            await DeleteUserParameters(userId);
            var sql = "insert into public.\"VshghCoachBotDB\"(\"height\", \"weight\",\"userId\" )"
                + $"values (@height, @weight, @userId )";


            NpgsqlCommand comm = new NpgsqlCommand(sql, con);

            comm.Parameters.AddWithValue("height", height);
            comm.Parameters.AddWithValue("weight", weight);
            comm.Parameters.AddWithValue("userId", userId);
            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();

        }
        public async Task<(double height, double weight)?> GetUserParameters(long userId)
        {
            var sql = "SELECT height, weight FROM public.\"VshghCoachBotDB\" WHERE \"userId\" = @userId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("userId", userId);

            await con.OpenAsync();
            var reader = await comm.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                double height = reader.GetDouble(0);
                double weight = reader.GetDouble(1);
                await con.CloseAsync();
                return (height, weight);
            }
            await con.CloseAsync();
            return null;
        }




        public async Task InsertUserSBD(long userId, double squat, double bench, double deadlift)
        {

            await DeleteUserSBD(userId);
            var sql = "insert into public.\"SBD\"(\"squat\", \"bench\",\"deadlift\",\"userId\" )"
                + $"values (@squat, @bench, @deadlift, @userId )";


            NpgsqlCommand comm = new NpgsqlCommand(sql, con);

            comm.Parameters.AddWithValue("squat", squat);
            comm.Parameters.AddWithValue("bench", bench);
            comm.Parameters.AddWithValue("deadlift", deadlift);
            comm.Parameters.AddWithValue("userId", userId);
            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();

        }
        
        public async Task<(double squat, double bench, double deadlift)?> GetUserSBD(long userId)
        {
            var sql = "SELECT squat, bench, deadlift FROM public.\"SBD\" WHERE \"userId\" = @userId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("userId", userId);

            await con.OpenAsync();
            var reader = await comm.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                double squat = reader.GetDouble(0);
                double bench = reader.GetDouble(1);
                double deadlift = reader.GetDouble(2);
                await con.CloseAsync();
                return (squat, bench, deadlift);
            }
            await con.CloseAsync();
            return null;
        }
        public async Task DeleteUserParameters(long userId)
        {
            var sql = "DELETE FROM public.\"VshghCoachBotDB\" WHERE \"userId\" = @userId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("userId", userId);

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }
        public async Task DeleteUserSBD(long userId)
        {
            var sql = "DELETE FROM public.\"SBD\" WHERE \"userId\" = @userId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("userId", userId);

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }
    }
}








