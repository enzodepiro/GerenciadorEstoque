using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Data;
using InventoryApp.Models;

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Controle de Estoque - Menu");
            Console.WriteLine("1 - Cadastrar produto");
            Console.WriteLine("2 - Listar produtos");
            Console.WriteLine("3 - Incrementar quantidade");
            Console.WriteLine("4 - Decrementar quantidade");
            Console.WriteLine("5 - Gerar script SQL CREATE TABLE");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha: ");
            var opt = Console.ReadLine();

            if (opt == "1") RegisterProduct(db);
            else if (opt == "2") ListProducts(db);
            else if (opt == "3") ChangeQuantity(db, true);
            else if (opt == "4") ChangeQuantity(db, false);
            else if (opt == "5") ShowCreateTableScript();
            else if (opt == "0") break;
            else Console.WriteLine("Opção inválida.");
        }
    }

    static void RegisterProduct(AppDbContext db)
    {
        Console.Write("Nome: ");
        var nome = Console.ReadLine() ?? "";
        Console.Write("Preço (ex: 19.90): ");
        if (!decimal.TryParse(Console.ReadLine(), out var preco))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        var p = new Product { Nome = nome, Preco = preco, Quantidade = 0 };
        db.Products.Add(p);
        db.SaveChanges();
        Console.WriteLine($"Produto cadastrado com id {p.IdProduto}.");
    }

    static void ListProducts(AppDbContext db)
    {
        var produtos = db.Products.OrderBy(p => p.IdProduto).ToList();
        if (!produtos.Any())
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }
        Console.WriteLine();
        Console.WriteLine("Id | Nome (preço) | Quantidade");
        foreach (var p in produtos)
            Console.WriteLine($"{p.IdProduto} | {p.Nome} ({p.Preco:F2}) | {p.Quantidade}");
    }

    static void ChangeQuantity(AppDbContext db, bool incrementar)
    {
        Console.Write("Id do produto: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }
        var p = db.Products.Find(id);
        if (p == null) { Console.WriteLine("Produto não encontrado."); return; }
        Console.Write("Quantidade a alterar: ");
        if (!int.TryParse(Console.ReadLine(), out var q)) { Console.WriteLine("Quantidade inválida."); return; }
        if (!incrementar && p.Quantidade - q < 0) { Console.WriteLine("Operação inválida: estoque não pode ficar negativo."); return; }
        p.Quantidade += (incrementar ? q : -q);
        db.SaveChanges();
        Console.WriteLine("Quantidade atualizada.");
    }

    static void ShowCreateTableScript()
    {
        Console.WriteLine();
        Console.WriteLine("Script SQL solicitado:");
        Console.WriteLine(@"
CREATE TABLE produto (
  id_produto INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  nome VARCHAR(45) NOT NULL,
  preco DECIMAL(10,2) NOT NULL,
  quantidade INT DEFAULT 0
);");
    }
}
