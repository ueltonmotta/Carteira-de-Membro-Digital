# 07 — Plano de Testes de Software

## Objetivo

Garantir a estabilidade das funcionalidades críticas e a segurança dos dados dos membros da igreja.

## Cenários de Teste

### TC-01 — Segurança no Registo

| Campo | Detalhe |
|---|---|
| **Cenário** | Tentativa de cadastro com perfil elevado usando código inválido |
| **Pré-condição** | Sistema em funcionamento, formulário de cadastro acessível |
| **Ação** | Selecionar perfil "Secretaria" e inserir um código de acesso inválido |
| **Resultado Esperado** | Sistema bloqueia o registo e exibe a mensagem "Código inválido" |
| **Critério de Aprovação** | Nenhum registro é criado no banco de dados |

---

### TC-02 — Controle de Acesso por Status

| Campo | Detalhe |
|---|---|
| **Cenário** | Membro com status "Pendente" tenta visualizar a carteirinha |
| **Pré-condição** | Conta criada, status ainda não aprovado pela secretaria |
| **Ação** | Membro faz login e navega para a tela de Carteirinha |
| **Resultado Esperado** | Cartão exibe marca d'água "Aguardando Aprovação" sem QR Code funcional |
| **Critério de Aprovação** | QR Code não é gerado; informações sensíveis não são expostas |

---

### TC-03 — Geolocalização Bloqueada

| Campo | Detalhe |
|---|---|
| **Cenário** | Membro tenta fazer check-in com permissão de GPS negada |
| **Pré-condição** | Evento ativo existente; GPS do navegador bloqueado |
| **Ação** | Clicar em "Assinar Lista" com permissões de geolocalização recusadas |
| **Resultado Esperado** | Disparo de alerta JavaScript informando que o acesso ao GPS foi negado |
| **Critério de Aprovação** | Nenhuma presença é registrada; membro é orientado a agir |

---

### TC-04 — Alerta Pastoral Automático

| Campo | Detalhe |
|---|---|
| **Cenário** | Membro ausente em 3 Santas Ceias consecutivas sem justificativa |
| **Pré-condição** | 3 eventos do tipo "Santa Ceia" registrados; membro sem check-in e sem justificativas |
| **Ação** | Secretaria acessa o Dashboard administrativo |
| **Resultado Esperado** | Ícone vermelho e pulsante "Visita Pastoral" exibido na Dashboard |
| **Critério de Aprovação** | Nome do membro aparece na lista de alertas pastorais |

---

## Matriz de Rastreabilidade

| Caso de Teste | Requisito Funcional | Módulo |
|---|---|---|
| TC-01 | Segurança no cadastro de admins | Autenticação |
| TC-02 | Controle de acesso por status | Carteirinha |
| TC-03 | Check-in via GPS | Geolocalização |
| TC-04 | Alerta de cuidado pastoral | Painel Administrativo |
