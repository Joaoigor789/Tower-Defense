# Security Checklist - Backend.TowerDefense

## 🔐 Antes de Fazer Deploy

### Secrets e Configurações
- [ ] **JWT Secret Key** está em variável de ambiente (não hardcoded)?
- [ ] **Database password** está em variável de ambiente?
- [ ] **Connection string** não está commitada no Git?
- [ ] Arquivo `appsettings.Production.json` está no `.gitignore`?

### Dependências
- [ ] Rodei `dotnet list package --vulnerable` para checar pacotes vulneráveis?
- [ ] Atualizei pacotes NuGet para versões mais recentes?
- [ ] Snyk scan passou sem vulnerabilidades críticas?

### Código
- [ ] Removi todos os `Console.WriteLine` com dados sensíveis?
- [ ] Não há senhas ou API keys hardcoded no código?
- [ ] TruffleHog scan passou (sem secrets vazados)?
- [ ] SecurityCodeScan não reportou vulnerabilidades?

### Infraestrutura
- [ ] HTTPS está habilitado em produção?
- [ ] Security Headers estão configurados (HSTS, X-Frame-Options, CSP)?
- [ ] Rate Limiting está ativo no endpoint de login?
- [ ] CORS está configurado apenas para domínios confiáveis?

### Docker
- [ ] Imagem Docker foi escaneada com Trivy?
- [ ] Não estou usando `latest` tag (uso versão específica)?
- [ ] Imagem base é de fonte confiável (Microsoft oficial)?

## 🧪 Testes de Segurança Locais

### 1. Scan de Secrets
```bash
# Instalar TruffleHog
pip install trufflehog

# Rodar scan
trufflehog filesystem . --only-verified
```

### 2. Scan de Dependências
```bash
# Vulnerabilidades em pacotes NuGet
dotnet list package --vulnerable

# Snyk (requer conta gratuita)
snyk test
```

### 3. Scan de Container
```bash
# Build da imagem
docker build -t tower-defense-api .

# Scan com Trivy
trivy image tower-defense-api
```

### 4. SAST (Análise de Código)
```bash
# SecurityCodeScan roda automaticamente no build
dotnet build
```

## 🚨 Se Encontrar uma Vulnerabilidade

1. **NÃO commite** o código vulnerável
2. Documente o problema
3. Consulte `SECURITY.md` para reportar
4. Aguarde fix antes de fazer merge

## 📚 Recursos

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [.NET Security Best Practices](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Snyk Vulnerability Database](https://snyk.io/vuln/)

---

**Segurança é responsabilidade de todos!** 🛡️
