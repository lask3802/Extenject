# GitHub Actions 工作流程說明 / GitHub Actions Workflow Guide

## 概述 / Overview

本文件說明為 Extenject 效能優化專案新增的自動化測試工作流程。

This document explains the automated testing workflow added for the Extenject performance optimization project.

---

## 工作流程檔案 / Workflow File

**路徑 / Path**: `.github/workflows/performance-test.yml`

---

## 功能特色 / Features

### 1. 自動觸發 / Automatic Triggers

工作流程會在以下情況自動執行:

The workflow automatically runs when:

- **Pull Request**: 修改以下檔案時 / When modifying:
  - `UnityProject/Assets/Plugins/Zenject/Source/**/*.cs` (原始碼)
  - `UnityProject/Assets/Plugins/Zenject/Tests/**/*.cs` (測試)
  - `.github/workflows/performance-test.yml` (工作流程本身)

- **Push**: 推送到以下分支時 / When pushing to:
  - `copilot/performance-optimize`
  - `performance-optimize`

- **Manual**: 透過 GitHub Actions UI 手動觸發 / Manual trigger via GitHub Actions UI

### 2. 測試執行 / Test Execution

#### Job 1: performance-tests (EditMode)

執行單元測試，包含:

Runs unit tests including:

- 效能基準測試 (Performance baseline tests)
- 元數據快取行為測試 (Metadata cache behavior tests)
- LINQ 消除正確性測試 (LINQ elimination correctness tests)
- 執行緒安全測試 (Thread safety tests)

**測試模式 / Test Mode**: EditMode  
**Unity 版本 / Unity Version**: 6000.0.64f1

#### Job 2: performance-integration-tests (PlayMode)

執行整合測試，包含:

Runs integration tests including:

- 複雜場景解析測試 (Complex resolve scenario tests)
- 跨元件整合測試 (Cross-component integration tests)

**測試模式 / Test Mode**: PlayMode  
**Unity 版本 / Unity Version**: 6000.0.64f1

### 3. 自動化報告 / Automated Reporting

#### GitHub Step Summary

每次執行都會產生摘要，包含:

Each run generates a summary including:

- ✅ 測試執行狀態 (通過/失敗數量) / Test execution status (passed/failed counts)
- 📊 效能測試類別清單 / Performance test categories list
- 📈 預期效能改善表格 / Expected performance improvements table
- 🔗 測試成果連結 / Test artifact links

#### PR 自動留言 / Automatic PR Comments

針對 Pull Request，工作流程會自動留言顯示:

For Pull Requests, the workflow automatically comments with:

- 測試結果摘要 (總計/通過/失敗) / Test results summary (total/passed/failed)
- 執行的測試類別 / Test categories executed
- 預期效能影響 / Expected performance impact
- 成果下載連結 / Artifact download links

### 4. 測試成果 / Test Artifacts

每次執行都會上傳以下成果:

Each run uploads the following artifacts:

- **Performance-Test-Results-{version}**: EditMode 測試結果
- **Integration-Performance-Results-{version}**: PlayMode 測試結果

成果包含:
- XML 測試報告 / XML test reports
- 詳細日誌 / Detailed logs
- 效能指標資料 / Performance metrics data

---

## 使用方式 / How to Use

### 查看工作流程執行 / View Workflow Runs

1. 前往 GitHub repository
2. 點擊 "Actions" 標籤
3. 選擇 "Performance Testing" 工作流程
4. 查看最新執行結果

### 手動觸發測試 / Manually Trigger Tests

1. 前往 "Actions" → "Performance Testing"
2. 點擊 "Run workflow" 按鈕
3. 選擇分支 (branch)
4. 點擊 "Run workflow" 確認

### 查看測試結果 / View Test Results

#### 方法 1: GitHub Step Summary

1. 點擊工作流程執行
2. 查看 "Summary" 頁面
3. 檢視自動產生的測試摘要

#### 方法 2: PR 留言

1. 開啟相關的 Pull Request
2. 向下捲動查看工作流程留言
3. 檢視測試結果和效能分析

#### 方法 3: 下載成果

1. 在工作流程執行頁面
2. 向下捲動到 "Artifacts" 區塊
3. 下載測試結果檔案
4. 解壓縮並檢視 XML 報告

---

## 測試分類 / Test Categories

所有效能測試都標記為 `[Category("Performance")]`:

All performance tests are tagged with `[Category("Performance")]`:

### 單元測試 / Unit Tests

1. **TestResolvePerformanceBaseline.cs**
   - 測試項目: 6 個
   - 目的: 建立效能基準
   - 測量: 解析時間、配置量

2. **TestLinqElimination.cs**
   - 測試項目: 7 個
   - 目的: 驗證 LINQ 移除後的正確性
   - 測量: 行為一致性

3. **TestInjectMetadataCache.cs**
   - 測試項目: 10 個
   - 目的: 驗證元數據快取行為
   - 測量: 快取命中率、參考一致性

4. **TestMetadataCacheThreadSafety.cs**
   - 測試項目: 5 個
   - 目的: 驗證執行緒安全
   - 測量: 並行存取正確性

### 整合測試 / Integration Tests

5. **TestPerformanceIntegration.cs**
   - 測試項目: 4 個
   - 目的: 驗證複雜場景
   - 測量: 端到端效能

---

## 預期效能改善 / Expected Performance Improvements

| 指標 / Metric | 改善 / Improvement |
|---------------|-------------------|
| Resolve Performance | ~10-20% faster |
| GC Allocations | ~30-40% reduction |
| Startup Time | ~5-10% faster |

---

## 疑難排解 / Troubleshooting

### 工作流程失敗 / Workflow Fails

**問題**: 測試執行失敗  
**解決方式**:
1. 檢查工作流程日誌
2. 查看失敗的測試案例
3. 本地執行相同測試驗證
4. 檢查 Unity 版本相容性

### 成果無法下載 / Artifacts Not Available

**問題**: 找不到測試成果  
**解決方式**:
1. 確認工作流程執行完成
2. 檢查 "Artifacts" 區塊
3. 等待成果上傳完成（可能需要數分鐘）

### PR 沒有收到留言 / PR Not Commented

**問題**: Pull Request 沒有自動留言  
**解決方式**:
1. 確認 PR 觸發了工作流程
2. 檢查工作流程權限設定
3. 查看工作流程執行日誌中的留言步驟

---

## 維護建議 / Maintenance Recommendations

### 定期更新 / Regular Updates

- 每季度檢查 Unity 版本更新
- 更新 GitHub Actions 版本
- 檢視效能趨勢

### 效能基準追蹤 / Performance Baseline Tracking

建議定期執行並記錄:
- 解析時間基準
- 記憶體配置基準
- 啟動時間基準

### 測試擴展 / Test Expansion

未來可考慮新增:
- 更多效能壓力測試
- 記憶體洩漏檢測
- 大規模依賴圖測試

---

## 相關文件 / Related Documentation

- [PERFORMANCE_OPTIMIZATION.md](../PERFORMANCE_OPTIMIZATION.md) - 完整效能優化說明
- [PERFORMANCE_OPTIMIZATION_ZH.md](../PERFORMANCE_OPTIMIZATION_ZH.md) - 中文版說明
- [Unity Test Runner Documentation](https://docs.unity3d.com/Manual/testing-editortestsrunner.html)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

## 聯絡資訊 / Contact Information

如有問題或建議，請:
- 開啟 GitHub Issue
- 在 Pull Request 中留言
- 聯絡專案維護者

For questions or suggestions:
- Open a GitHub Issue
- Comment on the Pull Request
- Contact project maintainers
