# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in **Backend.TowerDefense**, please report it responsibly.

### How to Report

**Email**: [Adicionar seu email aqui]

**What to include:**
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

### Response Time

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Fix Timeline**: Depends on severity (Critical: 24-48h, High: 7 days, Medium: 30 days)

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Security Measures

This project implements:
- ✅ JWT Authentication with BCrypt password hashing
- ✅ SQL Injection protection (EF Core parameterized queries)
- ✅ HTTPS enforcement
- ✅ CORS configuration
- ✅ Security Headers (HSTS, X-Frame-Options, CSP)
- ✅ Rate Limiting on authentication endpoints
- ✅ Automated security scanning (Snyk, Trivy, TruffleHog)

## Known Limitations

- JWT tokens do not support refresh tokens yet
- No rate limiting on non-auth endpoints (planned)

## Security Best Practices for Contributors

1. **Never commit secrets** (API keys, passwords, connection strings)
2. **Use environment variables** for sensitive configuration
3. **Run security scans** before submitting PRs
4. **Follow OWASP Top 10** guidelines

---

**Thank you for helping keep Backend.TowerDefense secure!** 🛡️
