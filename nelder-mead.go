package main

import (
	"fmt"
	"math"
	"sort"

	_ "github.com/Knetic/govaluate"
	"github.com/Pramod-Devireddy/go-exprtk"
)

const (
	defaultMaxIterations = 10000
	// reflection coefficient
	defaultAlpha = 1.0
	// contraction coefficient
	defaultBeta = 0.5
	// shrink coefficient
	defaultDelta = 0.5
	// expansion coefficient
	defaultGamma = 2.0

	epsilon = 1e-8
)

var expression string

// Optimizer represents the parameters to the Nelder-Mead simplex method.
type Optimizer struct {
	// Maximum number of iterations.
	MaxIterations int
	// Reflection coefficient.
	Alpha,
	// Contraction coefficient.
	Beta,
	// Shrink coefficient.
	Delta,
	// Expansion coefficient.
	Gamma float64
}

// New returns a new instance of Optimizer with all values set to the defaults.
func New() *Optimizer {
	return &Optimizer{
		MaxIterations: defaultMaxIterations,
		Alpha:         defaultAlpha,
		Beta:          defaultBeta,
		Delta:         defaultDelta,
		Gamma:         defaultGamma,
	}
}

func (o *Optimizer) CalculateNextPoints(
	objfunc func([]float64) float64,
	start [][]float64,
) ([]float64, [][]float64) {
	n := len(start)
	values := make([]float64, n)
	centroid := make([]float64, n-1) //mid
	ref := make([]float64, n-1)      //reflection
	exp := make([]float64, n-1)      //expansion
	contr := make([]float64, n-1)    //contraction

	for i := 0; i < n; i++ {
		values[i] = objfunc(start[i])
	}
	sort.Sort(nmVertexSorter{start, values})
	for i := 0; i < len(start[0]); i++ {
		cent := 0.0
		for m := 0; m < n-1; m++ {

			cent += start[m][i]
		}
		centroid[i] = cent / float64(n-1)
	}

	//reflect the largest value to new vertex ref
	reflectPoint(centroid, start[len(start)-1], ref, o.Alpha)
	refected_f := objfunc(ref)

	if refected_f < values[len(values)-2] && refected_f >= values[0] {
		//start[len(start)-1] = ref
		for i := 0; i < n-1; i++ {
			start[len(start)-1][i] = ref[i]
		}
		values[len(start)-1] = refected_f
	}

	//expansion
	if refected_f < values[0] {
		expandPoint(centroid, ref, exp, o.Gamma)
		expansion_f := objfunc(exp)
		if expansion_f < refected_f {
			for i := 0; i < n-1; i++ {
				start[len(start)-1][i] = exp[i]
			}
			values[len(start)-1] = expansion_f
		} else {
			for i := 0; i < n-1; i++ {
				start[len(start)-1][i] = ref[i]
			}
			values[len(start)-1] = refected_f
		}
	}

	//contraction
	if refected_f >= values[len(values)-2] {
		if refected_f < values[len(start)-1] {

			// perform outside contraction
			contractPoint(centroid, start[len(start)-1], ref, contr, o.Beta, false)

		} else {
			// perform inside contraction
			contractPoint(centroid, start[len(start)-1], ref, contr, o.Beta, true)

		}

		contraction_f := objfunc(contr)
		if contraction_f < values[len(start)-1] {
			for i := 0; i < n-1; i++ {
				start[len(start)-1][i] = contr[i]
			}
			values[len(start)-1] = contraction_f
		} else {
			//shrink
			for j := 1; j < n; j++ {
				for i := 0; i < len(start[0]); i++ {
					start[j][i] = start[0][i] + o.Delta*(start[j][i]-start[0][i])
				}
			}

		}

	}

	sort.Sort(nmVertexSorter{start, values})

	return values, start

}

func (o *Optimizer) checkStopCondition(values []float64) bool {
	s := standert_deviation(values)
	return s < epsilon
}

func reflectPoint(centroid, start, ref []float64, alpha float64) {
	for i := 0; i < len(centroid); i++ {
		ref[i] = centroid[i] + alpha*(centroid[i]-start[i])
	}
}

func expandPoint(centroid, ref, exp []float64, gamma float64) {
	for i := 0; i < len(centroid); i++ {
		exp[i] = centroid[i] + gamma*(ref[i]-centroid[i])
	}
}

func contractPoint(centroid, start, ref, contr []float64, beta float64, isInside bool) {
	if isInside {
		for i := 0; i < len(centroid); i++ {
			contr[i] = centroid[i] - beta*(centroid[i]-start[i])
		}
	} else {
		for i := 0; i < len(centroid); i++ {
			contr[i] = centroid[i] + beta*(ref[i]-centroid[i])
		}
	}
}

func standert_deviation(values []float64) float64 {
	n := float64(len(values))
	fsum := 0.0
	for i := 0; i < len(values); i++ {
		fsum += values[i]
	}
	favg := fsum / (n + 1)
	s := 0.0
	for i := 0; i < len(values); i++ {
		s += math.Pow((values[i]-favg), 2.0) / float64(len(values)-1)
	}
	return math.Sqrt(s)
}

type nmVertexSorter struct {
	vertices [][]float64
	values   []float64
}

func (n nmVertexSorter) Len() int {
	return len(n.values)
}

func (n nmVertexSorter) Less(i, j int) bool {
	return n.values[i] < n.values[j]
}

func (n nmVertexSorter) Swap(i, j int) {
	n.values[i], n.values[j] = n.values[j], n.values[i]
	n.vertices[i], n.vertices[j] = n.vertices[j], n.vertices[i]
}

func main() {
	var input string //"100*pow(X2-pow(X1, 2), 2) + pow((1-X1), 2)"
	fmt.Scan(&input)
	expression = input
	f := func(x []float64) float64 {

		exprtkObj := exprtk.NewExprtk()
		defer exprtkObj.Delete()

		exprtkObj.SetExpression(expression)

		exprtkObj.AddDoubleVariable("X1")
		exprtkObj.AddDoubleVariable("X2")

		err := exprtkObj.CompileExpression()
		if err != nil {
			fmt.Println(err.Error())

		}

		exprtkObj.SetDoubleVariableValue("X1", x[0])
		exprtkObj.SetDoubleVariableValue("X2", x[1])

		return exprtkObj.GetEvaluatedValue()
	}
	start := [][]float64{[]float64{1, 1}, []float64{2, 1}, []float64{-1, 2}}
	res, coord := New().CalculateNextPoints(f, start)
	fmt.Printf("%.3f %.3f", res, coord)
}
