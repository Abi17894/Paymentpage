(function () {
    'use strict';

    angular
        .module('angle')
        .controller('MstPaymentController', MstPaymentController);

    MstPaymentController.$inject = ['$rootScope', '$scope', '$state', 'AuthenticationService', '$modal', 'ScopeValueService', '$http', 'SocketService', 'Notify', '$location', 'apiManage', 'SweetAlert', '$route', 'ngTableParams'];

    function MstPaymentController($rootScope, $scope, $state, AuthenticationService, $modal, ScopeValueService, $http, SocketService, Notify, $location, apiManage, SweetAlert, $route, ngTableParams) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'MstPaymentController';
        $scope.lsoneapipage = $location.search().lsoneapipage;
        var lsoneapipage = $scope.lsoneapipage;
        activate();

        function activate() {
            var url = 'api/MstApplication360/GetPayment';
            lockUI();
            SocketService.get(url).then(function (resp) {
                $scope.payment_data = resp.data.application_list;
                unlockUI();
            });
        }

        $scope.addPayment = function () {
            var modalInstance = $modal.open({
                templateUrl: '/addPayment.html',
                controller: ModalInstanceCtrl,
                backdrop: 'static',
                keyboard: false,
                size: 'md'
            });
            ModalInstanceCtrl.$inject = ['$scope', '$modalInstance'];
            function ModalInstanceCtrl($scope, $modalInstance) {

                $scope.ok = function () {
                    $modalInstance.close('closed');
                };
                $scope.submit = function () {

                    var params = {
                        payment_name: $scope.txtPayment_name,
                        payment_code: $scope.txtPayment_code,
                        amount: $scope.txtAmount,
                        ifsc_code: $scope.txtIFSCCode,
                        account_name: $scope.txtAccountNumber,
                        confirm_account_name: $scope.txtConfirmAccountNumber
                    };
                    var url = 'api/MstApplication360/CreatePayment';
                    lockUI();
                    SocketService.post(url, params).then(function (resp) {
                        unlockUI();
                        if (resp.data.status == true) {
                            Notify.alert(resp.data.message, {
                                status: 'success',
                                pos: 'top-center',
                                timeout: 3000
                            });
                            $modalInstance.close('closed');
                            activate();
                        }
                        else {
                            Notify.alert(resp.data.message, {
                                status: 'warning',
                                pos: 'top-center',
                                timeout: 3000
                            });
                        }
                    });

                    $modalInstance.close('closed');
                }
            }
        }

        $scope.editPayment = function (payment_gid) {
            var modalInstance = $modal.open({
                templateUrl: '/editPayment.html',
                controller: ModalInstanceCtrl,
                backdrop: 'static',
                keyboard: false,
                size: 'md'
            });
            ModalInstanceCtrl.$inject = ['$scope', '$modalInstance'];
            function ModalInstanceCtrl($scope, $modalInstance) {
                var params = {
                    payment_gid: payment_gid
                };
                var url = 'api/MstApplication360/EditPayment';
                SocketService.getparams(url, params).then(function (resp) {

                    $scope.txteditPayment_name = resp.data.payment_name;
                    $scope.txteditPayment_code = resp.data.payment_code;
                    $scope.txteditAmount = resp.data.amount;
                    $scope.txteditIFSCCode = resp.data.ifsc_code;
                    $scope.txteditAccountName = resp.data.account_name;
                    $scope.txteditConfirmAccountName = resp.data.confirm_account_name;
                    $scope.payment_gid = resp.data.payment_gid;

                    // Disable non-editable fields
                    $scope.isPaymentNameDisabled = true;
                    $scope.isPaymentCodeDisabled = true;
                });

                $scope.ok = function () {
                    $modalInstance.close('closed');
                };

                $scope.update = function () {

                    var url = 'api/MstApplication360/UpdatePayment';
                    var params = {
                        payment_name: $scope.txteditPayment_name,
                        payment_code: $scope.txteditPayment_code,
                        amount: $scope.txteditAmount,
                        ifsc_code: $scope.txteditIFSCCode,
                        account_name: $scope.txteditAccountName,
                        confirm_account_name: $scope.txteditConfirmAccountName,
                        payment_gid: $scope.payment_gid
                    };
                    SocketService.post(url, params).then(function (resp) {
                        if (resp.data.status == true) {
                            $modalInstance.close('closed');
                            Notify.alert(resp.data.message, {
                                status: 'success',
                                pos: 'top-center',
                                timeout: 3000
                            });
                            activate();
                        } else {
                            $modalInstance.close('closed');
                            Notify.alert(resp.data.message, {
                                status: 'warning',
                                pos: 'top-center',
                                timeout: 3000
                            });
                        }
                    });
                    $modalInstance.close('closed');
                }
            }
        }

        $scope.Status_update = function (payment_gid) {
            var modalInstance = $modal.open({
                templateUrl: '/statusPayment.html',
                controller: ModalInstanceCtrl,
                backdrop: 'static',
                keyboard: false,
                size: 'md'
            });
            ModalInstanceCtrl.$inject = ['$scope', '$modalInstance'];
            function ModalInstanceCtrl($scope, $modalInstance) {

                var params = {
                    payment_gid: payment_gid
                };
                var url = 'api/MstApplication360/EditPayment';
                SocketService.getparams(url, params).then(function (resp) {
                    $scope.payment_gid = resp.data.payment_gid;
                    $scope.txtpayment_name = resp.data.payment_name;
                    $scope.rbo_status = resp.data.Status;
                });

                $scope.ok = function () {
                    $modalInstance.close('closed');
                };
                $scope.update_status = function () {

                    var params = {
                        payment_gid: payment_gid,
                        remarks: $scope.txtremarks,
                        rbo_status: $scope.rbo_status
                    };
                    var url = 'api/MstApplication360/InactivePayment';
                    lockUI();
                    SocketService.post(url, params).then(function (resp) {
                        unlockUI();
                        if (resp.data.status == true) {
                            Notify.alert(resp.data.message, {
                                status: 'success',
                                pos: 'top-center',
                                timeout: 3000
                            });
                        } else {
                            Notify.alert(resp.data.message, {
                                status: 'info',
                                pos: 'top-center',
                                timeout: 3000
                            });
                        }
                        activate();
                    });

                    $modalInstance.close('closed');
                }

                var param = {
                    payment_gid: payment_gid
                };

                var url = 'api/MstApplication360/PaymentInactiveLogview';
                lockUI();
                SocketService.getparams(url, param).then(function (resp) {
                    $scope.paymentinactivelog_data = resp.data.application_list;
                    unlockUI();
                });
            }
        }

        $scope.delete = function (payment_gid) {
            var params = {
                payment_gid: payment_gid
            };

            SweetAlert.swal({
                title: 'Are you sure?',
                text: 'Do You Want To Delete the Record?',
                showCancelButton: true,
                confirmButtonColor: '#DD6B55',
                confirmButtonText: 'Yes, delete it!',
                closeOnConfirm: false
            }, function (isConfirm) {
                if (isConfirm) {
                    lockUI();
                    var url = 'api/MstApplication360/DeletePayment';
                    SocketService.getparams(url, params).then(function (resp) {
                        unlockUI();
                        if (resp.data.status == true) {
                            SweetAlert.swal('Deleted Successfully!');
                            activate();
                        } else {
                            alert(resp.data.message, {
                                status: 'warning',
                                pos: 'top-center',
                                timeout: 3000
                            });
                            activate();
                            unlockUI();
                        }
                    });
                }
            });
        }
    }
})();
