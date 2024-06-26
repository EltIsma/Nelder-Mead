﻿using System.Windows.Documents;
using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;

namespace Nelder_Mead_App;

public class FlowDocumentLogger : ILogger
{
    FlowDocument _document;

    public FlowDocumentLogger(FlowDocument document)
    {
        _document = document;
    }

    public void LogSolution(Point solution)
    {
        _document.Dispatcher.Invoke(() =>
            {
                _document.Blocks.Add(new Paragraph(new Run($" ")));

                var paragraph = new Paragraph(new Bold(new Run($"РЕШЕНИЕ")));
                paragraph.Inlines.Add(new Run($"\nЗначение: {solution.Value}"));
                paragraph.Inlines.Add(new Run($"\nКоординаты: {Helpers.FormatCoordinates(solution.Coordinates)}"));

                _document.Blocks.Add(paragraph);
            });
    }

    public void LogStep(Simplex message, int steps)
    {
        _document.Dispatcher.Invoke(() =>
        {
            _document.Blocks.Add(new Paragraph(new Bold(new Run($"Шаг {steps}"))));
            _document.Blocks.Add(new Paragraph(new Bold(new Run($"Симплекс"))));
            var itemList = new List();
            foreach (var point in message.Points)
            {
                string formatted = $"{point.Value} - {Helpers.FormatCoordinates(point.Coordinates)}";
                itemList.ListItems.Add(new ListItem(new Paragraph(new Run(formatted))));
            }
            _document.Blocks.Add(itemList);

            var paragraph = new Paragraph(new Bold(new Run($"Лучшая точка")));
            string best = $"\n{message.Lowest.Value} - {Helpers.FormatCoordinates(message.Lowest.Coordinates)}";
            paragraph.Inlines.Add(new Run(best));
            _document.Blocks.Add(paragraph);

            paragraph = new Paragraph(new Bold(new Run($"Хорошая точка")));
            string good = $"\n{message.NextHighest.Value} - {Helpers.FormatCoordinates(message.NextHighest.Coordinates)}";
            paragraph.Inlines.Add(new Run(good));
            _document.Blocks.Add(paragraph);

            paragraph = new Paragraph(new Bold(new Run($"Худшая точка")));
            string worst = $"\n{message.Highest.Value} - {Helpers.FormatCoordinates(message.Highest.Coordinates)}";
            paragraph.Inlines.Add(new Run(worst));
            _document.Blocks.Add(paragraph);
        });
    }
}
