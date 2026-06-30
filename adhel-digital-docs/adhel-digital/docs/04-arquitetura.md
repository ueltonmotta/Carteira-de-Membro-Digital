# 04 — Arquitetura da Solução

## Padrão Arquitetural: MVC

O sistema utiliza o padrão **MVC (Model-View-Controller)**, garantindo separação limpa de responsabilidades entre as camadas da aplicação.

```
┌─────────────────────────────────────────────────────┐
│                   CLIENTE (Browser)                  │
│              Smartphone / Mobile-First               │
└───────────────────────┬─────────────────────────────┘
                        │  HTTP Request / Response
┌───────────────────────▼─────────────────────────────┐
│                   CONTROLLER                         │
│  ContaController | PainelController | HomeController │
│         (Regras de negócio e roteamento)             │
└──────────┬────────────────────────┬─────────────────┘
           │                        │
┌──────────▼──────────┐  ┌──────────▼──────────────────┐
│        MODEL        │  │           VIEW               │
│  Usuario.cs         │  │  Páginas .cshtml (Razor)     │
│  Evento.cs          │  │  HTML5 + CSS3 + JavaScript   │
│  Presenca.cs        │  │  FontAwesome Icons           │
│  Justificativa.cs   │  └─────────────────────────────┘
└──────────┬──────────┘
           │  Entity Framework Core (ORM)
┌──────────▼──────────┐
│     SQL SERVER      │
│   Base de Dados     │
└─────────────────────┘
```

## Descrição das Camadas

### Model
Representa a estrutura dos dados e as regras de domínio:
- `Usuario.cs` — Entidade de membros, secretaria e pastores
- `Evento.cs` — Cultos, reuniões e Santas Ceias
- `Presenca.cs` — Registro de check-ins por GPS
- `Justificativa.cs` — Justificações de ausências

### View
Interface visual entregue ao utilizador:
- Páginas `.cshtml` renderizadas server-side com **Razor**
- Responsividade mobile-first com `max-width: 414px`
- JavaScript puro para funcionalidades interativas (Dark Mode, QR Code, GPS)

### Controller
O "cérebro" da aplicação — processa requisições e aplica as regras de negócio:

| Controller | Responsabilidade |
|---|---|
| `ContaController` | Cadastro, login, logout, aprovação de membros |
| `PainelController` | Dashboard administrativo, alertas pastorais |
| `HomeController` | Carteirinha, check-in, agenda |

## Fluxo de Requisição

```
1. Membro abre o browser no smartphone
2. Envia requisição HTTP para o servidor
3. Controller recebe e processa a requisição
4. Consulta ou persiste dados via Entity Framework Core
5. SQL Server executa as queries
6. Controller retorna os dados para a View
7. Razor renderiza o HTML final
8. Browser exibe a interface ao membro
```

## Comunicação Cliente-Servidor

- **Protocolo:** HTTP/HTTPS
- **Autenticação:** Session-based com cookies
- **Check-in GPS:** Envio de `Latitude` e `Longitude` via formulário oculto (POST)
- **ORM:** Entity Framework Core com Code First Migrations
