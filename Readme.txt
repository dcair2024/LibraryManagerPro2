
📝 README.md
# 📚 LibraryManagerPro

**LibraryManagerPro** é uma aplicação web completa para gerenciamento de livros e autores, construída com **ASP.NET Core MVC** e **Entity Framework Core**.  
O projeto foi desenvolvido como parte de estudo e portfólio para demonstrar boas práticas de desenvolvimento, autenticação com Identity, CRUD completo e integração com API de geração de imagens.

---

## 🚀 Funcionalidades

✅ **Cadastro de Autores e Livros** (CRUD completo)  
✅ **Relacionamento N:N** entre livros e autores  
✅ **Autenticação e Autorização** com **ASP.NET Identity**  
✅ **Área administrativa** para gerenciar cadastros (somente para usuários Admin)  
✅ **Integração com API externa** (Flask) para geração automática de capas  
✅ **Fallback inteligente** usando Unsplash para gerar capas mesmo offline  
✅ **Banco de dados persistente** com **SQLite**  
✅ **Seed de dados** (3 autores e 3 livros iniciais) apenas se o banco estiver vazio  

---

## 🛠️ Tecnologias Utilizadas

- **C# 10 / .NET 6**
- **ASP.NET Core MVC**
- **Entity Framework Core (Code-First + Migrations)**
- **SQLite**
- **ASP.NET Identity**
- **Bootstrap 5**
- **Flask API (para geração de imagens)**
- **Unsplash (fallback de imagens)**

---

## 📷 Demonstração

| 📚 Lista de Livros | 📖 Detalhes do Livro |
|-----------------|-----------------|
| ![Lista de Livros](./screenshots/lista-livros.png) | ![Detalhes do Livro](./screenshots/detalhes-livro.png) |

| ➕ Criar Novo Livro | 🔑 Login Admin |
|--------------------|---------------|
| ![Criar Novo Livro](./screenshots/criar-livro.png) | ![Login Admin](./screenshots/login-admin.png) |


## 🏗️ Estrutura do Projeto



LibraryManagerPro/
├── Controllers/
│ ├── LivrosController.cs
│ └── AutoresController.cs
├── Models/
│ ├── Livro.cs
│ ├── Autor.cs
│ └── ViewModels/
├── Data/
│ ├── LibraryContext.cs
│ └── ApplicationDbContext.cs
├── Services/
│ ├── IImageService.cs
│ └── FakeImageService.cs
├── Views/
│ ├── Livros/
│ ├── Autores/
│ └── Shared/
└── Program.cs


---

## 🧑‍💻 Como Rodar o Projeto

1. **Clone o repositório**

```bash
git clone https://github.com/dcair2023/LibraryManagerPro.git
cd LibraryManagerPro

Restaure as dependências

dotnet restore


Rode o projeto

dotnet run


Acesse no navegador

https://localhost:7251

🔑 Usuário Admin Padrão

Usuário	Senha
cairo@teste.com	C@iro123

⚠️ Importante: altere a senha no ambiente de produção.

🌟 Diferenciais do Projeto

Código limpo e organizado seguindo boas práticas de arquitetura

Uso de injeção de dependência para serviços

Seeding de dados condicional para evitar sobrescrever o banco

Capas de livros geradas automaticamente — experiência mais realista

Pronto para ser usado como projeto de portfólio

🧾 Licença

Este projeto está sob a licença MIT. Sinta-se livre para usar e melhorar.

📌 Autor: Davi Santana Cairo

💡 "Só devo me preocupar com aquilo que eu posso mudar."