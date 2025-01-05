// Inicjalizacja tooltipów
$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// Inicjalizacja toastr
toastr.options = {
    "closeButton": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "timeOut": "3000"
};

// Walidacja IMEI
function validateIMEI(imei) {
    if (!/^\d{15}$/.test(imei)) {
        return false;
    }

    let sum = 0;
    let alternate = false;

    for (let i = imei.length - 1; i >= 0; i--) {
        let d = parseInt(imei.charAt(i));
        if (alternate) {
            d *= 2;
            if (d > 9) {
                d -= 9;
            }
        }
        sum += d;
        alternate = !alternate;
    }

    return (sum % 10 === 0);
}

// Dynamiczne aktualizacje statusu
function updateDeviceStatus(deviceId, status) {
    fetch('/Device/UpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `id=${deviceId}&status=${status}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success('Status został zaktualizowany');
            } else {
                toastr.error('Wystąpił błąd podczas aktualizacji statusu');
            }
        });
}

// Formatowanie numeru telefonu
function formatPhoneNumber(input) {
    let number = input.value.replace(/\D/g, '');
    let formatted = '';

    if (number.length > 0) {
        formatted += number.substr(0, 3);
        if (number.length > 3) {
            formatted += '-' + number.substr(3, 3);
        }
        if (number.length > 6) {
            formatted += '-' + number.substr(6, 3);
        }
    }

    input.value = formatted;
}

// Obsługa formularzy
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
});