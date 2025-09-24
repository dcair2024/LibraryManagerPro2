
# 📚 LibraryManagerPro

Sistema de gerenciamento de biblioteca feito em **ASP.NET Core MVC + EF Core**.  
Este projeto é parte do meu portfólio e demonstra CRUD completo, autenticação com Identity, e **geração automática de capas de livros usando OpenAI DALL·E**.

---

## ✨ Funcionalidades
- ✅ Cadastro, edição e exclusão de livros
- ✅ Associação de autores aos livros (N:N)
- ✅ Login e gerenciamento de usuários com Identity
- ✅ **Geração de capa automática via IA**
- ✅ Interface responsiva com Bootstrap

---

## 🚀 Tecnologias Usadas
- ASP.NET Core 9
- Entity Framework Core
- SQLite
- Identity
- OpenAI API (DALL·E)
- Bootstrap 5

---

## 🖼️ Demonstração

| Página | Exemplo |
|-------|---------|
| **Index de Livros** | ![Index Screenshot](./docs/screenshots/index.png) |
| **Formulário de Criação** | ![Create Screenshot](./docs/screenshots/create.png) |

---

## ⚙️ Configuração Local

1. Clone o repositório  
   ```bash
   git clone https://github.com/dcair2024/LibraryManagerPro.git
Crie o arquivo appsettings.Development.json na raiz do projeto:

{
  "OpenAI": {
    "ApiKey": "sua-chave-aqui"
  }
}


Rode as migrations e inicie:

dotnet ef database update
dotnet run


Acesse em http://localhost:5000

📸 Screenshots

Coloque os screenshots em docs/screenshots e use o mesmo nome que está no README.

🧠 Autor

Feito com 💻 e ☕ por Davi Santana Cairo
🔗 Meu LinkedIn


---

Quer que eu prepare **o arquivo `.gitignore` completo para .NET** (já com o appsettings.Development ignorado) e te mande aqui para você só copiar e colar?  
Assim você evita subir binários, cache e arquivos de config sensíveis.