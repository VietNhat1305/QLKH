<script>
import Layout from "@/layouts/main";
import PageHeader from "@/components/page-header";
import {numeric, required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {pagingModel} from "@/models/pagingModel";
import Multiselect from "vue-multiselect";
import {baiThiModel} from "@/models/baiThiModel";
import DatePicker from "vue2-datepicker";
import Treeselect from "@riophae/vue-treeselect";
export default {
  page: {
    title: "Quản lý khách hàng",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, PageHeader, DatePicker},
  data() {
    return {
      title: "Quản lý khách hàng",
      items: [
        {
          text: "Khách hàng",
          href: '/bai-thi'
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      data: [],
      fields: [
        { key: 'STT',
          label: 'STT',
          class: 'td-stt',
          sortable: false,
          thStyle: "text-align:center",
        },
        {
          key: "hoLot",
          label: "Họ và tên",
          class: 'td-name',
          sortable: false,
          thStyle: "text-align:center",
        },
        // {
        //   key: "name",
        //   label: "Tên",
        //   class: 'td-ten',
        //   sortable: false,
        //   thStyle: "text-align:center",
        // },
        {
          key: "taiKhoan",
          label: "Tài khoản",
          class: 'td-taikhoan',
          thStyle: "text-align:center",
          sortable: false,
        },
        {
          key: "diemSo",
          label: "Điểm số",
          class: 'td-diemSo',
          thStyle: "text-align:center",
          sortable: true,
          thClass: 'hidden-sortable'
        },
        {
          key: "theDiem",
          label: "Thẻ điểm",
          class: 'td-thediem',
          thStyle: "text-align:center",
          sortable: false,
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly',
          sortable: false,
        }
      ],
      currentPage: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      showModal: false,
      showDeleteModal: false,
      submitted: false,
      sortBy: "age",
      filter: null,
      sortDesc: false,
      filterOn: [],
      numberOfElement: 1,
      totalRows: 1,
      model: baiThiModel.baseJson(),
      pagination: pagingModel.baseJson(),
      theDiem: [
        { id: 1, name: "Vàng", loaiThe: "A", diemCanTren: "100", diemCanDuoi: "61" },
        { id: 2, name: "Bạc", loaiThe: "B", diemCanTren: "60", diemCanDuoi: "31" },
        { id: 3, name: "Đồng", loaiThe: "C", diemCanTren: "30", diemCanDuoi: "0" }
      ]
    };
  },
  created() {
  },
  methods: {
    async fnGetList() {
      this.$refs.tblList?.refresh()
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("baiThiStore/delete",{id :  this.model.id}).then((res) => {
          if (res.code===0) {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          var a = {
            message: res.message,
            code: res.code
          };
          //   console.log("LOG A : " ,a)
          this.$store.dispatch("snackBarStore/addNotify", {
            message: res.message,
            code: res.code
          });
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;

        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          console.log("LOG USER UPDATE NE : " , this.model)
          // Update model
          await this.$store.dispatch("baiThiStore/update", this.model).then((res) => {
            if (res.code === 0) {
              this.showModal = false;
              this.$refs.tblList.refresh();
            }
            this.$store.dispatch("snackBarStore/addNotify", {
              message: res.message,
              code: res.code,
            });
          });
        } else {
          // Create model
          console.log("LOG USER CREATE NE : " , this.model)
          await this.$store.dispatch("baiThiStore/create", this.model).then((res) => {
            if (res.code === 0) {
              this.fnGetList();
              this.showModal = false;
              this.model={}
            }
            this.$store.dispatch("snackBarStore/addNotify", {
              message: res.message,
              code: res.code,
            });
          });
        }
        loader.hide();
      this.submitted = false;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("baiThiStore/getById", {id : id}).then((res) => {
        if (res.code===0) {
          console.log(res)
          // this.model = baiThiModel.toJson(res.data);
          const data = baiThiModel.fromJson(res.data);
          this.model = {
            ...data,
            ngayThamGia: data.ngayThamGia ? new Date(data.ngayThamGia) : null,
          };
          console.log(this.model)
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", {
            message: res.message,
            code: res.code,
          });
        }
      });
    },
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("baiThiStore/getPagingParams", params)
        return promise.then(resp => {
          let items = resp.data.data
          this.totalRows = resp.data.totalRows
          this.numberOfElement = resp.data.data.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false
      }
    },
    async handleIncrease(id) {
      await this.$store.dispatch("baiThiStore/getById", {id : id}).then((res) => {
        if (res.code===0) {
          console.log(res)
          this.model = baiThiModel.toJson(res.data);
          // Tăng giá trị của diemSo lên 5
          this.model.diemSo += 5;

          // Lưu giá trị mới của diemSo
          this.$store.dispatch("baiThiStore/update", this.model).then((res) => {
            if (res.code === 0) {
              this.fnGetList(); // Tải lại danh sách bảng dữ liệu
              this.model={};
            }
            var a = {
              message: res.message,
              code: res.code
            };
            this.$store.dispatch("snackBarStore/addNotify", {
              message: res.message,
              code: res.code
            });
          }).catch((err) => {
            console.error("Lỗi khi cập nhật điểm số:", err);
          });
        } else {
          this.$store.dispatch("snackBarStore/addNotify", {
            message: res.message,
            code: res.code,
          });
        }
      });

    },
  },
  mounted() {
  },
  watch: {
    model: {
      deep: true,
      handler(val) {
        // addCoQuanToModel()
        // this.saveValueToLocalStorage()
      }
    },
    showModal(status) {
      if (status == false) this.model = baiThiModel.baseJson();
    },
    // model() {
    //   return this.model;
    // },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    }
  },
};
</script>

<template>
  <Layout>
    <PageHeader :title="title" :items="items"/>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">
                    <input
                        v-model="filter"
                        type="text"
                        class="form-control"
                        placeholder="Tìm kiếm ..."
                    />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-button
                    type="button"
                    class="btn-label cs-btn-success mb-2 me-2 css-button"
                    @click="showModal = true"
                    size="md"
                    style="border: none;"
                  >
                    <i class="mdi mdi-plus-box label-icon"></i> Thêm
                  </b-button>
                  <b-modal
                      v-model="showModal"
                      title="Thông tin khách hàng"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer"
                    >
                      <div class="row">
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Họ lót</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="userName"
                                v-model.trim="model.hoLot"
                                type="text"
                                class="form-control"
                                placeholder="Nhập họ lót"

                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Tên</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="lastName"
                                v-model="model.name"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tên"
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Tài khoản</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="lastName"
                                v-model="model.taiKhoan"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tài khoản"
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Ngày tham gia</label>
                            <span style="color: red">&nbsp;*</span>
                            <date-picker
                                v-model="model.ngayThamGia"
                                format="DD/MM/YYYY"
                                id="percent"
                                placeholder="Nhập ngày tham gia"
                            >
                            </date-picker>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Địa chỉ khách hàng</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <textarea
                                id="lastName"
                                v-model="model.diaChiKhachHang"
                                type="text"
                                class="form-control"
                                placeholder="Nhập địa chỉ khách hàng"
                            >
                            </textarea>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Điểm số</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="lastName"
                                v-model="model.diemSo"
                                disabled="true"
                                type="number"
                                class="form-control"
                                placeholder="Nhập điểm số"
                            />
                          </div>
                        </div>


                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" @click="showModal = false" class="border-0">
                          Đóng
                        </b-button>
                        <b-button  type="submit" variant="success" class="ms-1 btn-add">Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mt-4">
                  <div class="col-sm-12 col-md-6">
                    <div
                        class="col-sm-12 d-flex justify-content-left align-items-center"
                    >
                      <div
                          id="tickets-table_length"
                          class="dataTables_length m-1"
                          style="
                        display: flex;
                        justify-content: left;
                        align-items: center;
                      "
                      >
                        <div class="me-1" >Hiển thị </div>
                        <b-form-select
                            class="form-select form-select-sm"
                            v-model="perPage"
                            size="sm"
                            :options="pageOptions"
                            style="width: 70px"
                        ></b-form-select
                        >&nbsp;
                        <div style="width: 100px"> dòng </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                  <b-table
                      class="datatables table-admin"
                      :items="myProvider"
                      :fields="fields"
                      striped
                      bordered
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    <template v-slot:cell(hoLot)="data">
                          <span>
                              {{data.item.hoLot}} {{data.item.name}}
                          </span>
                    </template>
                    <template v-slot:cell(trangThai)="data">
                          <span style="margin-left: 5px">
                              {{data.item.trangThai.name}}
                          </span>
                    </template>
                    <template v-slot:cell(theDiem)="data">
                      <div v-if="data.item.diemSo <= 30 && data.item.diemSo >= 0"
                        style="background-color: #8b4513; border-radius: 5px; padding: 5px; text-align: center;"
                      >
                        <span style="margin-left: 5px; color: #fff; font-weight: bold;">
                            Đồng
                        </span>
                      </div>
                      <div v-else-if="data.item.diemSo <= 60 && data.item.diemSo >= 31"
                        style="background-color: #b0c4de; border-radius: 5px; padding: 5px; text-align: center;"
                      >
                        <span style="margin-left: 5px; color: #fff; font-weight: bold;">
                            Bạc
                        </span>
                      </div>
                      <div v-else
                        style="background-color: #FFDF00; border-radius: 5px; padding: 5px; text-align: center;"
                      >
                        <span style="margin-left: 5px; color: #000; font-weight: bold;">
                            Vàng
                        </span>
                      </div>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-outline btn-sm" 
                          @click="handleIncrease(data.item.id)">
                        <i class="fas fa-plus text-success me-1"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-outline btn-sm"
                          v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt text-success me-1"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-outline btn-sm"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt text-danger me-1"></i>
                      </button>
                    </template>
                  </b-table>
                </div>
                <div class="row mt-3">
                  <b-col>
                    <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <!-- pagination -->
                        <b-pagination
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>

              </div>
            </div>
            <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style>
.td-xuly {
  text-align: center;
  width: 20%
}
</style>
