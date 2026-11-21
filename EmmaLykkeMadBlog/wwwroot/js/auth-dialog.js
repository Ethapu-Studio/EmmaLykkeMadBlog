(() => {
    // What the overlay shows when the server redirects back with ?auth=<code>.
    const MESSAGES = {
        'login-failed':     { view: 'login',    kind: 'error', text: 'E-mailen eller adgangskoden er forkert, eller kontoen er ikke bekræftet endnu.' },
        'unavailable':      { view: 'login',    kind: 'error', text: 'Der er et teknisk problem lige nu. Prøv igen om lidt.' },
        'rate-limited':     { view: 'login',    kind: 'error', text: 'For mange forsøg. Vent lidt, og prøv igen.' },
        'required':         { view: 'login',    kind: 'info',  text: 'Log ind for at fortsætte.' },
        'confirmed':        { view: 'login',    kind: 'info',  text: 'Din e-mail er bekræftet. Du kan nu logge ind.' },
        'confirm-failed':   { view: 'login',    kind: 'error', text: 'Linket er ugyldigt eller udløbet. Opret kontoen igen for at få et nyt link.' },
        'reset-ok':         { view: 'login',    kind: 'info',  text: 'Din adgangskode er ændret. Log ind med den nye.' },
        'reset-failed':     { view: 'forgot',   kind: 'error', text: 'Linket er ugyldigt eller udløbet. Bed om et nyt her.' },
        'register-invalid': { view: 'register', kind: 'error', text: 'Tjek, at e-mailen er gyldig, og at adgangskoden er mindst 7 tegn.' },
        'register-sent':    { view: 'sent' },
        'forgot-sent':      { view: 'sent' }
    };

    const getDialog = () => document.querySelector('[data-auth-dialog]');

    function showMessage(dialog, view, message) {
        dialog.querySelectorAll('.auth-msg').forEach(el => {
            el.hidden = true;
            el.textContent = '';
        });

        if (!message) return;

        const el = dialog.querySelector(`[data-auth-view="${view}"] .auth-msg`);
        if (!el) return;

        el.dataset.kind = message.kind;
        el.textContent = message.text;
        el.hidden = false;
    }

    function setView(dialog, view, message) {
        dialog.dataset.view = view;
        dialog.querySelectorAll('[data-auth-view]').forEach(section => {
            section.hidden = section.dataset.authView !== view;
        });

        // After sign-in the visitor returns to the page they were on.
        dialog.querySelectorAll('[data-return-url]').forEach(input => {
            input.value = location.pathname;
        });

        showMessage(dialog, view, message);
        dialog.querySelector(`[data-auth-view="${view}"] input:not([type="hidden"])`)?.focus();
    }

    function openFromAddress() {
        const params = new URLSearchParams(location.search);
        const code = params.get('auth');
        if (!code) return;

        // Remove the code from the address so a refresh does not show the message again.
        params.delete('auth');
        const query = params.toString();
        history.replaceState(null, '', location.pathname + (query ? '?' + query : '') + location.hash);

        const dialog = getDialog();
        const entry = MESSAGES[code];
        if (!dialog || !entry) return;

        setView(dialog, entry.view, entry.text ? entry : null);
        if (!dialog.open) dialog.showModal();
    }

    document.addEventListener('click', event => {
        const dialog = getDialog();
        if (!dialog) return;

        const opener = event.target.closest('[data-auth-open]');
        if (opener) {
            event.preventDefault();
            setView(dialog, opener.dataset.authOpen);
            if (!dialog.open) dialog.showModal();
            return;
        }

        const switcher = event.target.closest('[data-auth-switch]');
        if (switcher) {
            setView(dialog, switcher.dataset.authSwitch);
            return;
        }

        if (event.target.closest('[data-auth-close]') || event.target === dialog) {
            dialog.close(); // the second case is a click on the blurred backdrop
        }
    });

    // Close the account menu on an outside click or Esc.
    document.addEventListener('click', event => {
        document.querySelectorAll('details.account[open]').forEach(menu => {
            if (!menu.contains(event.target)) menu.open = false;
        });
    });

    document.addEventListener('keydown', event => {
        if (event.key !== 'Escape') return;
        document.querySelectorAll('details.account[open]').forEach(menu => {
            menu.open = false;
            menu.querySelector('summary')?.focus();
        });
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', openFromAddress);
    } else {
        openFromAddress();
    }
})();