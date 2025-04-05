// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initialize all Bootstrap components properly
document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.forEach(function (popoverTriggerEl) {
        new bootstrap.Popover(popoverTriggerEl);
    });

    // Initialize modals with proper event handling
    var modalTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="modal"]'));
    modalTriggerList.forEach(function (modalTrigger) {
        modalTrigger.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            var targetId = this.getAttribute('data-bs-target');
            var targetModal = document.querySelector(targetId);
            
            if (targetModal) {
                var modal = bootstrap.Modal.getInstance(targetModal);
                if (!modal) {
                    modal = new bootstrap.Modal(targetModal, {
                        backdrop: 'static',
                        keyboard: false
                    });
                }
                modal.show();
            }
        });
    });

    // Handle modal forms
    document.querySelectorAll('.modal form').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.stopPropagation();
            var modal = bootstrap.Modal.getInstance(this.closest('.modal'));
            if (modal) {
                modal.hide();
            }
        });
    });

    // Handle modal close buttons
    document.querySelectorAll('.modal .btn-close, .modal .btn-secondary').forEach(function (closeBtn) {
        closeBtn.addEventListener('click', function (e) {
            e.preventDefault();
            var modal = bootstrap.Modal.getInstance(this.closest('.modal'));
            if (modal) {
                modal.hide();
            }
        });
    });
});

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        var alerts = document.querySelectorAll('.alert');
        alerts.forEach(function (alert) {
            var bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 5000);
});

// File input custom text
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.custom-file-input').forEach(function (input) {
        input.addEventListener('change', function (e) {
            var fileName = e.target.files[0].name;
            var next = e.target.nextElementSibling;
            next.innerText = fileName;
        });
    });
});

// Password strength indicator
document.addEventListener('DOMContentLoaded', function () {
    var passwordInputs = document.querySelectorAll('input[type="password"]');
    passwordInputs.forEach(function (input) {
        input.addEventListener('input', function (e) {
            var password = e.target.value;
            var strength = 0;
            
            if (password.length >= 8) strength++;
            if (password.match(/[a-z]+/)) strength++;
            if (password.match(/[A-Z]+/)) strength++;
            if (password.match(/[0-9]+/)) strength++;
            if (password.match(/[$@#&!]+/)) strength++;

            var progressBar = input.parentElement.querySelector('.password-strength');
            if (progressBar) {
                progressBar.style.width = (strength * 20) + '%';
                
                switch (strength) {
                    case 0:
                    case 1:
                        progressBar.className = 'password-strength progress-bar bg-danger';
                        break;
                    case 2:
                    case 3:
                        progressBar.className = 'password-strength progress-bar bg-warning';
                        break;
                    case 4:
                    case 5:
                        progressBar.className = 'password-strength progress-bar bg-success';
                        break;
                }
            }
        });
    });
});

// Form validation
document.addEventListener('DOMContentLoaded', function () {
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});

// Appointment date/time validation
document.addEventListener('DOMContentLoaded', function () {
    var dateInputs = document.querySelectorAll('input[type="date"]');
    dateInputs.forEach(function (input) {
        input.addEventListener('change', function (e) {
            var selectedDate = new Date(e.target.value);
            var today = new Date();
            today.setHours(0, 0, 0, 0);

            if (selectedDate < today) {
                input.setCustomValidity('Please select a future date');
            } else {
                input.setCustomValidity('');
            }
        });
    });
});

// Confirmation dialogs
document.addEventListener('DOMContentLoaded', function () {
    var confirmButtons = document.querySelectorAll('[data-confirm]');
    confirmButtons.forEach(function (button) {
        button.addEventListener('click', function (e) {
            if (!confirm(button.dataset.confirm)) {
                e.preventDefault();
            }
        });
    });
});

// Dynamic form fields
document.addEventListener('DOMContentLoaded', function () {
    var addFieldButtons = document.querySelectorAll('[data-add-field]');
    addFieldButtons.forEach(function (button) {
        button.addEventListener('click', function (e) {
            var template = document.querySelector(button.dataset.template);
            if (template) {
                var clone = template.content.cloneNode(true);
                var container = document.querySelector(button.dataset.container);
                if (container) {
                    container.appendChild(clone);
                }
            }
        });
    });
});

// AJAX form submission
function submitFormAsync(formElement, successCallback, errorCallback) {
    var formData = new FormData(formElement);
    
    fetch(formElement.action, {
        method: formElement.method,
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            if (successCallback) successCallback(data);
        } else {
            if (errorCallback) errorCallback(data);
        }
    })
    .catch(error => {
        if (errorCallback) errorCallback({ message: 'An error occurred while processing your request.' });
    });
}

// Image preview
document.addEventListener('DOMContentLoaded', function () {
    var imageInputs = document.querySelectorAll('input[type="file"][accept^="image/"]');
    imageInputs.forEach(function (input) {
        input.addEventListener('change', function (e) {
            var preview = input.parentElement.querySelector('.image-preview');
            if (preview && e.target.files && e.target.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = 'block';
                }
                reader.readAsDataURL(e.target.files[0]);
            }
        });
    });
});
