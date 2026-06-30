# 06 — Programação de Funcionalidades

## Módulos do Sistema

###  Autenticação — Cadastro com Termos

**Fluxo:**
1. Utilizador preenche o formulário de cadastro com seus dados pessoais
2. Concorda com o estatuto da igreja (checkbox obrigatório)
3. Sistema grava o registro com `Status = Pendente`
4. Perfis administrativos (`Secretaria`, `Pastor`) exigem um **código de validação** exclusivo

**Regra de Negócio:**
- Membros com `Status = Pendente` visualizam marca d'água na carteirinha
- Somente a Secretaria pode alterar o status para `Ativo`

---

###  Carteirinha — QR Code Dinâmico

**Geração:**
- QR Code gerado em JavaScript via canvas (`100px`)
- Dados codificados: ID do membro + hash de validação

**Validação Visual:**

| Situação | Aparência do QR Code |
|---|---|
| Credencial **ativa** | Código em preto, cartão normal |
| Credencial **expirada** | Código em vermelho, aviso de renovação |
| Membro **pendente** | Marca d'água "Aguardando Aprovação" |

---

###  Check-in — Assinatura via GPS

**Fluxo Técnico:**
1. Membro clica em "Assinar Lista" no culto ativo
2. Navegador solicita permissão de geolocalização ao utilizador
3. JavaScript captura `Latitude` e `Longitude` via `navigator.geolocation`
4. Coordenadas são inseridas em campos ocultos de um formulário
5. Formulário é enviado via `POST` para o servidor
6. Controller valida as coordenadas e registra a presença

**Tratamento de Erro:**
- Permissão negada → alerta JavaScript informando que o GPS foi recusado
- Geolocalização indisponível → mensagem orientando o membro a contatar a secretaria

---

###  Alerta Pastoral — Detecção de Ausências

**Lógica:**
- Consulta LINQ cruza as últimas **3 Santas Ceias** com a tabela `Presenca`
- Verifica se o membro está ausente em todas elas **sem justificativa** registrada
- Membros identificados disparam o indicador visual no painel

**Interface:**
- Ícone vermelho e **pulsante** ("Visita Pastoral") aparece no Dashboard da Secretaria
- Lista os nomes dos membros em alerta com data da última presença registrada

---

###  Justificativas

- Membro ausente pode submeter justificativa pelo sistema
- Justificativas aprovadas removem o alerta pastoral para aquele evento específico
- Secretaria visualiza e gerencia todas as justificativas pendentes

---

## Resumo por Módulo

| Módulo | Funcionalidade | Tecnologia Principal |
|---|---|---|
| Autenticação | Cadastro com validação de código para admins | C# / ASP.NET Core MVC |
| Carteirinha | QR Code dinâmico com validação visual | JavaScript / Canvas API |
| Check-in | Captura e envio de coordenadas GPS | `navigator.geolocation` / HTTP POST |
| Administração | Alerta pastoral com detecção LINQ | C# LINQ / Entity Framework Core |
| Dark Mode | Modo escuro persistente | JavaScript / `localStorage` |
