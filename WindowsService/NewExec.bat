RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Caja.In\Deploy
RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Caja.Out\Deploy
RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Copere.In\Deploy
RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Copere.Out\Deploy
RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Merge\Deploy
RD /S /Q C:\KS\ACMR\WinService\Ks.Batch.Reverse\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Caja.In\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Caja.Out\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Copere.In\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Copere.Out\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Merge\Deploy
MD C:\KS\ACMR\WinService\Ks.Batch.Reverse\Deploy
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Caja.In\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Caja.In\Deploy"
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Caja.Out\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Caja.Out\Deploy"
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Copere.In\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Copere.In\Deploy"
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Copere.Out\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Copere.Out\Deploy"
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Merge\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Merge\Deploy"
xcopy /s "D:\GitHub\KreierSolutions\WindowsService\Ks.Batch.Reverse\bin\Release" "C:\KS\ACMR\WinService\Ks.Batch.Reverse\Deploy"