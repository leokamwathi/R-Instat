﻿' Instat-R
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License k
' along with this program.  If not, see <http://www.gnu.org/licenses/>.
Imports instat.Translations

Public Class sdgOneVarCompareModels
    Private bControlsInitialised As Boolean = False
    Private clsRcdfcompFunction, clsRdenscompFunction, clsRqqcompFunction, clsRppcompFunction, clsListFunction, clsRAsDataFrame, clsModel, clsRGofStat, clsRReceiver, clsRsyntax, clsRPlotFunction, clsOperation As New RFunction
    Private clsOperatorforTable, clsOperatorForBreaks As New ROperator
    Private WithEvents ucrRecs As ucrReceiver
    Public bfirstload As Boolean = True

    Private Sub sdgOneVarCompareModels(sender As Object, e As EventArgs) Handles MyBase.Load
        autoTranslate(Me)
    End Sub

    Public Sub InitialiseControls()

        'ucrInputChiSquareBreaks
        ucrChkInputChiSquareBreakpoints.SetText("Input Chi-Square Breakpoints")

        'ucrChkPlots
        ucrChkCDF.SetParameter(New RParameter("ft", 1), bNewChangeParameterValue:=True, bNewAddRemoveParameter:=True, strNewValueIfChecked:="TRUE", strNewValueIfUnchecked:="FALSE")
        ucrChkCDF.SetRDefault("TRUE")
        ucrChkCDF.SetText("CDF")

        ucrChkDensity.SetParameter(New RParameter("ft", 1), bNewChangeParameterValue:=True, bNewAddRemoveParameter:=True, strNewValueIfChecked:="TRUE", strNewValueIfUnchecked:="FALSE")
        ucrChkDensity.SetText("Density")
        ucrChkDensity.SetRDefault("FALSE")

        ucrChkPP.SetParameter(New RParameter("ft", 1), bNewChangeParameterValue:=True, bNewAddRemoveParameter:=True, strNewValueIfChecked:="TRUE", strNewValueIfUnchecked:="FALSE")
        ucrChkPP.SetRDefault("FALSE")
        ucrChkPP.SetText("PP")

        ucrChkQQ.SetParameter(New RParameter("ft", 1), bNewChangeParameterValue:=True, bNewAddRemoveParameter:=True, strNewValueIfChecked:="TRUE", strNewValueIfUnchecked:="FALSE")
        ucrChkQQ.SetRDefault("FALSE")
        ucrChkQQ.SetText("QQ")
        InitialiseTabs()
        'ucrSaveGOF
        ucrSaveGOF.SetPrefix("GOF")
        ' ucrSaveGOF.SetSaveTypeAsModel() ' or graph?
        ucrSaveGOF.SetCheckBoxText("Save Fit")
        ucrSaveGOF.SetIsComboBox()
        ucrSaveGOF.SetAssignToIfUncheckedValue("last_model")

        'ucrSaveDisplayChi
        ucrSaveDisplayChi.SetPrefix("ChiSquare")
        ucrSaveDisplayChi.SetSaveTypeAsDataFrame()
        ucrSaveDisplayChi.SetCheckBoxText("Save DisplayChi")
        ucrSaveDisplayChi.SetIsComboBox()
        ucrSaveDisplayChi.SetAssignToIfUncheckedValue("last_model")

        'ucrSavePlot
        ucrSavePlots.SetPrefix("plots")
        ucrSavePlots.SetSaveTypeAsModel()
        ucrSavePlots.SetCheckBoxText("Save Plot")
        ucrSavePlots.SetIsComboBox()
        ucrSavePlots.SetAssignToIfUncheckedValue("last_model")
    End Sub

    Public Sub SetRCode(clsNewRGofStat As RFunction, clsNewReceiver As RFunction, clsNewRPlotFunction As RFunction, clsNewclsRAsDataFrame As RFunction, Optional clsNewOperatorforTable As ROperator = Nothing, Optional bReset As Boolean = False)
        If Not bControlsInitialised Then
            InitialiseControls()
        End If

        clsRGofStat = clsNewRGofStat
        clsRReceiver = clsNewReceiver
        clsOperatorforTable = clsNewOperatorforTable
        clsRAsDataFrame = clsNewclsRAsDataFrame
        clsRPlotFunction = clsNewRPlotFunction

        'Setting Rcode for the sub dialog
        ucrSaveGOF.SetRCode(clsRGofStat, bReset)
        ucrChkCDF.SetRCode(clsRPlotFunction, bReset)
        ucrChkDensity.SetRCode(clsRPlotFunction, bReset)
        ucrChkInputChiSquareBreakpoints.SetRCode(clsOperatorForBreaks, bReset)
        ucrChkPP.SetRCode(clsRPlotFunction, bReset)
        ucrChkQQ.SetRCode(clsRPlotFunction, bReset)
        ucrSaveDisplayChi.SetRCode(clsRAsDataFrame, bReset)

        If bReset Then
            tbpOneVarCompareModels.SelectedIndex = 0
        End If
    End Sub

    Public Sub CreateGraphs()
        Dim strTemp As String = ""

        If ucrChkCDF.Checked Then
            clsRPlotFunction.SetRCommand("fitdistrplus::cdfcomp")
            clsRPlotFunction.AddParameter("ft", clsRFunctionParameter:=clsRReceiver)
            frmMain.clsRLink.RunScript(clsRPlotFunction.ToScript(), 3)
        End If
        If ucrChkPP.Checked Then
            'clsRPlotFunction.ClearParameters()
            clsRPlotFunction.SetRCommand("fitdistrplus::denscomp")
            clsRPlotFunction.AddParameter("ft", clsRFunctionParameter:=clsRReceiver)
            frmMain.clsRLink.RunScript(clsRPlotFunction.ToScript(), 3)
        End If
        If ucrChkQQ.Checked Then
            'clsRPlotFunction.ClearParameters()
            clsRPlotFunction.SetRCommand("fitdistrplus::qqcomp")
            clsRPlotFunction.AddParameter("ft", clsRFunctionParameter:=clsRReceiver)
            frmMain.clsRLink.RunScript(clsRPlotFunction.ToScript(), 3)
        End If
        If ucrChkDensity.Checked Then
            ' clsRPlotFunction.ClearParameters()
            clsRPlotFunction.SetRCommand("fitdistrplus::ppcomp")
            clsRPlotFunction.AddParameter("ft", clsRFunctionParameter:=clsRReceiver)
            frmMain.clsRLink.RunScript(clsRPlotFunction.ToScript(), 3)
        End If

        If ucrSaveDisplayChi.IsComplete Then
            frmMain.clsRLink.RunScript(clsOperatorforTable.ToScript(), 0)
            clsRAsDataFrame.ToScript(strTemp)
            frmMain.clsRLink.RunScript(strTemp, 0)
        End If
        If ucrChkInputChiSquareBreakpoints.Checked Then
            frmMain.clsRLink.RunScript(clsOperatorForBreaks.ToScript(), 2)
        End If

    End Sub

    Public Sub DisplayChiBreaks()
        If ucrChkInputChiSquareBreakpoints.Checked Then
            clsOperatorForBreaks.SetOperation("$")
            clsOperatorForBreaks.AddParameter(iPosition:=0, clsRFunctionParameter:=clsModel)
            clsOperatorForBreaks.AddParameter(strParameterValue:="chisqbreaks")
        End If
    End Sub

    Private Sub InitialiseTabs()
        For i = 0 To tbpOneVarCompareModels.TabCount - 1
            tbpOneVarCompareModels.SelectedIndex = i
        Next
        tbpOneVarCompareModels.SelectedIndex = 0
    End Sub

End Class

