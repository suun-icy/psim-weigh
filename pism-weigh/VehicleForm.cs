using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;

namespace pism_weigh
{
    /// <summary>
    /// 车辆档案管理窗口
    /// </summary>
    public partial class VehicleForm : Form
    {
        private List<Vehicle> _vehicles;
        private Vehicle _editing;

        public VehicleForm()
        {
            InitializeComponent();
        }

        private void VehicleForm_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            var keyword = txtSearch.Text.Trim();
            _vehicles = string.IsNullOrEmpty(keyword)
                ? DatabaseHelper.GetAllVehicles()
                : DatabaseHelper.SearchVehicles(keyword);
            dgvVehicles.DataSource = null;
            dgvVehicles.DataSource = _vehicles;
            TranslateColumns(dgvVehicles);
            lblCount.Text = "共 " + _vehicles.Count + " 辆车";
            ClearEdit();
        }

        private void ClearEdit()
        {
            _editing = null;
            txtPlate.Text = "";
            txtVehicleType.SelectedIndex = -1;
            txtBrandModel.Text = "";
            txtRatedLoad.Text = "";
            txtCurbWeight.Text = "";
            txtOwnerName.Text = "";
            txtOwnerPhone.Text = "";
            txtOwnerUnit.Text = "";
            txtFuelType.SelectedIndex = -1;
            txtEmissionStandard.SelectedIndex = -1;
            dtpRegisteredDate.Value = DateTime.Today;
            txtRemark.Text = "";
            lblStatus.Text = "状态: -";
            btnSave.Text = "添加车辆";
            btnDelete.Visible = false;
        }

        private void dgvVehicles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVehicles.CurrentRow == null) return;
            var v = dgvVehicles.CurrentRow.DataBoundItem as Vehicle;
            if (v == null) return;

            _editing = v;
            txtPlate.Text = v.PlateNumber;
            txtVehicleType.Text = v.VehicleType ?? "";
            txtBrandModel.Text = v.BrandModel ?? "";
            txtRatedLoad.Text = v.RatedLoad > 0 ? v.RatedLoad.ToString("F2") : "";
            txtCurbWeight.Text = v.CurbWeight > 0 ? v.CurbWeight.ToString("F2") : "";
            txtOwnerName.Text = v.OwnerName ?? "";
            txtOwnerPhone.Text = v.OwnerPhone ?? "";
            txtOwnerUnit.Text = v.OwnerUnit ?? "";
            txtFuelType.Text = v.FuelType ?? "";
            txtEmissionStandard.Text = v.EmissionStandard ?? "";
            dtpRegisteredDate.Value = v.RegisteredDate ?? DateTime.Today;
            txtRemark.Text = v.Remark ?? "";
            lblStatus.Text = "状态: " + GetStatusText(v.Status);
            btnSave.Text = "保存修改";
            btnDelete.Visible = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var plate = txtPlate.Text.Trim();
            if (string.IsNullOrWhiteSpace(plate))
            {
                MessageBox.Show("车牌号不能为空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查车牌是否重复（新增时）
            if (_editing == null)
            {
                var exist = DatabaseHelper.GetVehicleByPlate(plate);
                if (exist != null)
                {
                    MessageBox.Show("车牌 '" + plate + "' 已存在。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            decimal ratedLoad, curbWeight;
            decimal.TryParse(txtRatedLoad.Text.Trim(), out ratedLoad);
            decimal.TryParse(txtCurbWeight.Text.Trim(), out curbWeight);

            var vehicle = _editing ?? new Vehicle();
            vehicle.PlateNumber = plate;
            vehicle.VehicleType = txtVehicleType.Text.Trim();
            vehicle.BrandModel = txtBrandModel.Text.Trim();
            vehicle.RatedLoad = ratedLoad;
            vehicle.CurbWeight = curbWeight;
            vehicle.OwnerName = txtOwnerName.Text.Trim();
            vehicle.OwnerPhone = txtOwnerPhone.Text.Trim();
            vehicle.OwnerUnit = txtOwnerUnit.Text.Trim();
            vehicle.FuelType = txtFuelType.Text.Trim();
            vehicle.EmissionStandard = txtEmissionStandard.Text.Trim();
            vehicle.RegisteredDate = dtpRegisteredDate.Value;
            vehicle.Remark = txtRemark.Text.Trim();
            vehicle.UpdateTime = DateTime.Now;

            if (DatabaseHelper.SaveVehicle(vehicle))
            {
                RefreshList();
                MessageBox.Show(_editing != null ? "修改成功。" : "添加成功。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("保存失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editing == null) return;
            if (MessageBox.Show("确定删除车辆 '" + _editing.PlateNumber + "' ？", "确认",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (DatabaseHelper.DeleteVehicle(_editing.Id))
            {
                RefreshList();
            }
            else
            {
                MessageBox.Show("删除失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var records = DatabaseHelper.GetAllWeighRecords();
                var plates = records
                    .Where(r => !string.IsNullOrWhiteSpace(r.PlateNumber))
                    .Select(r => r.PlateNumber.Trim())
                    .Distinct()
                    .ToList();

                int imported = 0;
                foreach (var plate in plates)
                {
                    var exist = DatabaseHelper.GetVehicleByPlate(plate);
                    if (exist == null)
                    {
                        var v = new Vehicle { PlateNumber = plate };
                        if (DatabaseHelper.SaveVehicle(v))
                            imported++;
                    }
                }
                RefreshList();
                MessageBox.Show("导入完成，新增 " + imported + " 辆车。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RefreshList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static string GetStatusText(string status)
        {
            switch (status)
            {
                case "Active": return "正常";
                case "Frozen": return "已冻结";
                case "Blacklisted": return "黑名单";
                default: return status ?? "-";
            }
        }

        private static void TranslateColumns(DataGridView dgv)
        {
            var map = new Dictionary<string, string>
            {
                {"PlateNumber", "车牌号"}, {"Province", "省份"}, {"PlateCode", "车牌号码"},
                {"VehicleType", "车辆类型"}, {"BrandModel", "品牌型号"},
                {"RatedLoad", "核定载重(吨)"}, {"CurbWeight", "整备质量(吨)"},
                {"OwnerName", "车主"}, {"OwnerPhone", "车主电话"}, {"OwnerUnit", "所属单位"},
                {"FuelType", "燃油类型"}, {"EmissionStandard", "排放标准"},
                {"RegisteredDate", "注册日期"}, {"Status", "状态"},
                {"Remark", "备注"}, {"CreateTime", "创建时间"}, {"UpdateTime", "更新时间"}
            };
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (map.ContainsKey(col.DataPropertyName))
                    col.HeaderText = map[col.DataPropertyName];
            }
        }
    }
}
