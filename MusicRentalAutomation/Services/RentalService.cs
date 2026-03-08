using System.ComponentModel;
using MusicRentalAutomation.Models;
using Npgsql;

namespace MusicRentalAutomation.Services;

public sealed class RentalService
{
    private readonly string _connectionString;

    public BindingList<Instrument> Instruments { get; } = new();
    public BindingList<Booking> Bookings { get; } = new();
    public BindingList<Contract> Contracts { get; } = new();
    public BindingList<Payment> Payments { get; } = new();

    public RentalService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string sql = """
            CREATE TABLE IF NOT EXISTS instruments (
                id SERIAL PRIMARY KEY,
                name TEXT NOT NULL,
                category TEXT NOT NULL,
                daily_rate NUMERIC(12,2) NOT NULL CHECK (daily_rate >= 0),
                status TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS bookings (
                id SERIAL PRIMARY KEY,
                instrument_id INTEGER NOT NULL REFERENCES instruments(id),
                client_full_name TEXT NOT NULL,
                start_date DATE NOT NULL,
                end_date DATE NOT NULL,
                booking_status TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS contracts (
                id SERIAL PRIMARY KEY,
                booking_id INTEGER NOT NULL REFERENCES bookings(id),
                contract_number TEXT NOT NULL UNIQUE,
                signed_at TIMESTAMP NOT NULL,
                total_amount NUMERIC(12,2) NOT NULL CHECK (total_amount >= 0),
                contract_status TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS payments (
                id SERIAL PRIMARY KEY,
                contract_id INTEGER NOT NULL REFERENCES contracts(id),
                amount NUMERIC(12,2) NOT NULL CHECK (amount >= 0),
                paid_at TIMESTAMP NOT NULL,
                method TEXT NOT NULL,
                payment_status TEXT NOT NULL
            );
            """;

        using var command = new NpgsqlCommand(sql, connection);
        command.ExecuteNonQuery();

        SeedInstruments(connection);
        RefreshAll();
    }

    public void RefreshAll()
    {
        LoadInstruments();
        LoadBookings();
        LoadContracts();
        LoadPayments();
    }

    public void AddInstrument(string name, string category, decimal dailyRate)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string sql = """
            INSERT INTO instruments (name, category, daily_rate, status)
            VALUES (@name, @category, @daily_rate, 'Доступен')
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("category", category);
        command.Parameters.AddWithValue("daily_rate", dailyRate);
        var id = Convert.ToInt32(command.ExecuteScalar());

        Instruments.Add(new Instrument
        {
            Id = id,
            Name = name,
            Category = category,
            DailyRate = dailyRate,
            Status = "Доступен"
        });
    }

    public Booking AddBooking(int instrumentId, string clientFullName, DateTime startDate, DateTime endDate)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string sql = """
            INSERT INTO bookings (instrument_id, client_full_name, start_date, end_date, booking_status)
            VALUES (@instrument_id, @client_full_name, @start_date, @end_date, 'Забронировано')
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("instrument_id", instrumentId);
        command.Parameters.AddWithValue("client_full_name", clientFullName);
        command.Parameters.AddWithValue("start_date", startDate.Date);
        command.Parameters.AddWithValue("end_date", endDate.Date);
        var id = Convert.ToInt32(command.ExecuteScalar());

        var booking = new Booking
        {
            Id = id,
            InstrumentId = instrumentId,
            ClientFullName = clientFullName,
            StartDate = startDate.Date,
            EndDate = endDate.Date,
            BookingStatus = "Забронировано"
        };

        Bookings.Add(booking);
        return booking;
    }

    public Contract CreateContract(Booking booking, decimal totalAmount)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var contractNumber = $"CTR-{DateTime.Now:yyyyMMddHHmmss}-{booking.Id}";

        const string sql = """
            INSERT INTO contracts (booking_id, contract_number, signed_at, total_amount, contract_status)
            VALUES (@booking_id, @contract_number, @signed_at, @total_amount, 'Действует')
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("booking_id", booking.Id);
        command.Parameters.AddWithValue("contract_number", contractNumber);
        command.Parameters.AddWithValue("signed_at", DateTime.Now);
        command.Parameters.AddWithValue("total_amount", totalAmount);
        var id = Convert.ToInt32(command.ExecuteScalar());

        var contract = new Contract
        {
            Id = id,
            BookingId = booking.Id,
            ContractNumber = contractNumber,
            SignedAt = DateTime.Now,
            TotalAmount = totalAmount,
            ContractStatus = "Действует"
        };

        Contracts.Add(contract);
        return contract;
    }

    public Payment RegisterPayment(int contractId, decimal amount, string method)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string sql = """
            INSERT INTO payments (contract_id, amount, paid_at, method, payment_status)
            VALUES (@contract_id, @amount, @paid_at, @method, 'Оплачен')
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("contract_id", contractId);
        command.Parameters.AddWithValue("amount", amount);
        command.Parameters.AddWithValue("paid_at", DateTime.Now);
        command.Parameters.AddWithValue("method", method);
        var id = Convert.ToInt32(command.ExecuteScalar());

        var payment = new Payment
        {
            Id = id,
            ContractId = contractId,
            Amount = amount,
            Method = method,
            PaidAt = DateTime.Now,
            PaymentStatus = "Оплачен"
        };

        Payments.Add(payment);
        return payment;
    }

    private void LoadInstruments()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT id, name, category, daily_rate, status FROM instruments ORDER BY id;", connection);
        using var reader = command.ExecuteReader();

        Instruments.Clear();
        while (reader.Read())
        {
            Instruments.Add(new Instrument
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                DailyRate = reader.GetDecimal(3),
                Status = reader.GetString(4)
            });
        }
    }

    private void LoadBookings()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT id, instrument_id, client_full_name, start_date, end_date, booking_status FROM bookings ORDER BY id;", connection);
        using var reader = command.ExecuteReader();

        Bookings.Clear();
        while (reader.Read())
        {
            Bookings.Add(new Booking
            {
                Id = reader.GetInt32(0),
                InstrumentId = reader.GetInt32(1),
                ClientFullName = reader.GetString(2),
                StartDate = reader.GetDateTime(3),
                EndDate = reader.GetDateTime(4),
                BookingStatus = reader.GetString(5)
            });
        }
    }

    private void LoadContracts()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT id, booking_id, contract_number, signed_at, total_amount, contract_status FROM contracts ORDER BY id;", connection);
        using var reader = command.ExecuteReader();

        Contracts.Clear();
        while (reader.Read())
        {
            Contracts.Add(new Contract
            {
                Id = reader.GetInt32(0),
                BookingId = reader.GetInt32(1),
                ContractNumber = reader.GetString(2),
                SignedAt = reader.GetDateTime(3),
                TotalAmount = reader.GetDecimal(4),
                ContractStatus = reader.GetString(5)
            });
        }
    }

    private void LoadPayments()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT id, contract_id, amount, paid_at, method, payment_status FROM payments ORDER BY id;", connection);
        using var reader = command.ExecuteReader();

        Payments.Clear();
        while (reader.Read())
        {
            Payments.Add(new Payment
            {
                Id = reader.GetInt32(0),
                ContractId = reader.GetInt32(1),
                Amount = reader.GetDecimal(2),
                PaidAt = reader.GetDateTime(3),
                Method = reader.GetString(4),
                PaymentStatus = reader.GetString(5)
            });
        }
    }

    private static void SeedInstruments(NpgsqlConnection connection)
    {
        using var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM instruments;", connection);
        var count = Convert.ToInt32(countCommand.ExecuteScalar());
        if (count > 0)
        {
            return;
        }

        const string seedSql = """
            INSERT INTO instruments (name, category, daily_rate, status)
            VALUES
                ('Yamaha C40', 'Гитара', 1200, 'Доступен'),
                ('Roland TD-07', 'Ударные', 3500, 'Доступен'),
                ('Casio CT-S1', 'Синтезатор', 1800, 'Доступен');
            """;

        using var seedCommand = new NpgsqlCommand(seedSql, connection);
        seedCommand.ExecuteNonQuery();
    }
}
