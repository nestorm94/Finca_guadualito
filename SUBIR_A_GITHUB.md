# Subir proyecto a GitHub

Repositorio: https://github.com/nestorm94/Finca_guadualito

## 1. Instalar Git (si no lo tienes)

Descarga: https://git-scm.com/download/win

Reinicia Cursor o PowerShell después de instalar.

## 2. Comandos (ejecutar en esta carpeta)

Abre PowerShell en:

`C:\Users\Asus\.cursor\projects\empty-window\InventarioGanadero`

```powershell
git init
git add .
git commit -m "Proyecto Registro Ganadero MVC con autenticación y menú lateral"
git branch -M main
git remote add origin https://github.com/nestorm94/Finca_guadualito.git
git push -u origin main
```

## 3. Autenticación GitHub

Si pide usuario/contraseña, usa un **Personal Access Token** (no la contraseña de GitHub):

1. GitHub → Settings → Developer settings → Personal access tokens
2. Generar token con permiso `repo`
3. Usuario: `nestorm94`, contraseña: el token

O inicia sesión con **GitHub Desktop**: https://desktop.github.com/

## 4. Verificar

Recarga https://github.com/nestorm94/Finca_guadualito — debe mostrar el código, no "empty repository".
