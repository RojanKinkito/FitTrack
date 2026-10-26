// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Give submitted forms a clear, lightweight busy state while the request is processed.
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form[method="post"]').forEach((form) => {
        form.addEventListener('submit', () => {
            const submitButton = form.querySelector('button[type="submit"]');

            if (!submitButton || form.dataset.submitting === 'true') {
                return;
            }

            form.dataset.submitting = 'true';
            submitButton.disabled = true;
            submitButton.setAttribute('aria-busy', 'true');
            submitButton.dataset.originalText = submitButton.textContent;
            submitButton.textContent = submitButton.textContent.trim() === 'Logout' ? 'Signing out…' : 'Saving…';
        });
    });
});
