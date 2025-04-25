$(document).ready(function () {
    window.onload = function () {
        CKEDITOR.replace('moTa');
        CKEDITOR.replace('contentmucluc');
    };
    function updateOrderStatus(orderId, status) {
        $.ajax({
            url: "/Admin/Order/changeStatusOrder",
            type: "POST",
            dataType: "json",
            data: {
                orderId: orderId,
                status: status
            },
            success: function (response) {
                if (response.isSuccess) {
                    Swal.fire({
                        title: 'Thành công!',
                        text: response.message,
                        icon: 'success',
                        confirmButtonText: 'OK',
                        timer: 3000,
                        timerProgressBar: true,
                    }).then(() => {
                        window.location.href = '/Admin/Order';
                    });
                }
            },
            error: function () {
                Swal.fire({
                    title: 'Lỗi!',
                    text: 'Không thể cập nhật trạng thái đơn hàng. Vui lòng thử lại sau.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    }

    $(document).on("click", ".btn-ChangeStatus", function (e) {
        e.preventDefault();
        var orderId = $(this).data('order');  // Lấy order ID từ data attribute của nút
        console.log("Order ID:", orderId);  // Kiểm tra xem Order ID được lấy đúng hay không

        Swal.fire({
            title: 'Cập nhật trạng thái đơn hàng',
            text: 'Chọn trạng thái bạn muốn cập nhật:',
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đang giao hàng',
            denyButtonText: 'Đã giao',
            cancelButtonText: 'Hủy đơn',
            showCloseButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                updateOrderStatus(orderId, 'dang_giao');
            } else if (result.isDenied) {
                updateOrderStatus(orderId, 'da_giao');
            } else if (result.isDismissed && result.dismiss === Swal.DismissReason.cancel) {
                updateOrderStatus(orderId, 'huy_don');
            }
        });
    });
    $(document).on("click", ".btn-delete-brand", function (e) {
        e.preventDefault();
        var id = $(this).data('productid');
        var row = $(this).closest('tr');
        Swal.fire({
            title: 'Bạn có muốn xóa nhãn hàng',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Xóa',
            showCloseButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Admin/Brand/deleteBrand",
                    type: "POST",
                    dataType: "json",
                    data: {
                        id: id,
                    },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                title: 'Xóa thành công!',
                                text: response.message,
                                icon: 'success',
                                confirmButtonText: 'OK',
                                timer: 3000,
                                timerProgressBar: true,
                            }).then((result) => {
                                // Check if the SweetAlert was confirmed
                                if (result.isConfirmed) {
                                    window.location.href = "/Admin/Brand/";
                                }
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            title: 'Lỗi!',
                            text: 'Không thể cập nhật trạng thái đơn hàng. Vui lòng thử lại sau.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                });
            }
        });
   });
});
