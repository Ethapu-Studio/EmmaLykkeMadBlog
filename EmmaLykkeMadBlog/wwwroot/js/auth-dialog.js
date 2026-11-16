(() => {
    const getDialog = () => document.querySelector('[data-auth-dialog]');

    function setView(dialog, view) {
        dialog.dataset.view = view;
        dialog.querySelectorAll('[data-auth-view]').forEach(section => {
            section.hidden = section.dataset.authView !== view;
        });
        dialog.querySelector(`[data-auth-view="${view}"] input`)?.focus();
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

    // Design phase only: stop the forms from posting until the backend exists.
    document.addEventListener('submit', event => {
        if (event.target.matches('form[data-preview]')) {
            event.preventDefault();
        }
    });
})();